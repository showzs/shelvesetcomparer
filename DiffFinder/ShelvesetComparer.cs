// <copyright file="ShelvesetComparer.cs" company="https://github.com/rajeevboobna/CompareShelvesets">Copyright https://github.com/rajeevboobna/CompareShelvesets. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>

using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DiffFinder
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ShelvesetComparer
    {
        /// <summary>
        /// Command ID for Result window
        /// </summary>
        public const int ShelvesetComparerResuldId = 0x0100;

        /// <summary>
        /// Command ID for TeamExplorer compare view
        /// </summary>
        public const int ShelvesetComparerTeamExplorerViewId = 0x0200;

        /// <summary>
        /// Dte Command name for Result window, keep in sync with vsct Button names (removing special characters and whitespaces)
        /// </summary>
        public const string ShelvesetComparerResuldIdDteCommandName = "Team.DiffFinderResults";

        /// <summary>
        /// Dte Command name for Result window, keep in sync with vsct Button names (removing special characters and whitespaces)
        /// </summary>
        public const string ShelvesetComparerTeamExplorerViewIdDteCommandName = "Team.DiffFinderSelect";

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("667E3733-CFA5-4E39-B8E0-8732F02CDB56");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShelvesetComparer"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ShelvesetComparer(AsyncPackage package)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            TraceOutput("Initializing Package ..");

            OleMenuCommandService commandService = this.ServiceProvider.GetService<IMenuCommandService, OleMenuCommandService>();
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, ShelvesetComparerResuldId);
                var menuItem = new MenuCommand(this.ShelvesetComparerResuldIdMenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(CommandSet, ShelvesetComparerTeamExplorerViewId);
                menuItem = new MenuCommand(this.ShelvesetComparerTeamExplorerViewIdMenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }

            TraceOutput("Package initialized.");
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ShelvesetComparer Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Static helper to ensure package is loaded and call Compare command
        /// </summary>
        public static void ExecuteCommand_Compare()
        {
            if (Instance != null)
            {
                Instance.ShowComparisonToolWindow();
            }
            else
            {
                ExecuteCommand(ShelvesetComparerResuldIdDteCommandName);
            }
        }

        /// <summary>
        /// Static helper to ensure package is loaded and call Select command
        /// </summary>
        public static void ExecuteCommand_Select()
        {
            if (Instance != null)
            {
                Instance.NavigateToShelvestComparerPage();
            }
            else
            {
                ExecuteCommand(ShelvesetComparerTeamExplorerViewIdDteCommandName);
            }
        }
        private static void ExecuteCommand(string commandName)
        {
            if (!ThreadHelper.CheckAccess())
            {
                ThreadHelper.JoinableTaskFactory.Run(async () =>
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    ExecuteCommand(commandName);
                });
            }

            ThreadHelper.ThrowIfNotOnUIThread();

            // if the package has not yet been initialized, then we need to call it via DTE
            var dte2 = Package.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            Microsoft.Assumes.NotNull(dte2);
            dte2.ExecuteCommand(commandName);
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(AsyncPackage package)
        {
            Instance = new ShelvesetComparer(package);
        }

        /// <summary>
        /// Open and show ShelvesetComparer result window.
        /// </summary>
        public void ShowComparisonToolWindow()
        {
            // Async ToolWindow implementation: https://github.com/microsoft/VSSDK-Analyzers/blob/main/doc/VSSDK003.md
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            _ = this.package.JoinableTaskFactory.RunAsync(async delegate
              {
                  ToolWindowPane window = await this.package.ShowToolWindowAsync(typeof(ShelvesetComparerToolWindow), 0, true, this.package.DisposalToken);
                  if ((null == window) || (null == window.Frame))
                  {
                      throw new NotSupportedException(Resources.CanNotCreateWindow);
                  }

                  await this.package.JoinableTaskFactory.SwitchToMainThreadAsync();
                  IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
                  Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
              });
        }

        /// <summary>
        /// Menu item callback for "Team - Results view" item.
        /// 
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void ShelvesetComparerResuldIdMenuItemCallback(object sender, EventArgs e)
        {
            this.ShowComparisonToolWindow();
        }

        /// <summary>
        /// Menu item callback for "Team - Select view" item.
        /// 
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void ShelvesetComparerTeamExplorerViewIdMenuItemCallback(object sender, EventArgs e)
        {
            NavigateToShelvestComparerPage();
        }

        /// <summary>
        /// Open ShelvesetComparer select page in TeamExplorer
        /// </summary>
        public void NavigateToShelvestComparerPage()
        {
            var teamExplorer = ServiceProvider.GetService<ITeamExplorer>();
            teamExplorer.NavigateToShelvesetComparer();
        }

        /// <summary>
        /// Write trace to output (only if trace is enabled)
        /// </summary>
        public void TraceOutput(string text)
        {
#if TRACE
            OutputPaneWriteLine($"TRACE: {text}");
#endif
        }

        /// <summary>
        /// Write to own output pane in output window with optional DateTime prefix and activate pane afterwards.
        /// </summary>
        public void OutputPaneWriteLine(string text, bool prefixDateTime = true)
        {
            OutputPaneWriteLine(this.ServiceProvider, text, prefixDateTime);
        }

        /// <summary>
        /// Write text with optional DateTime prefix to own output pane (create if not existing) and activate pane afterwards.
        /// </summary>
        public static void OutputPaneWriteLine(IServiceProvider serviceProvider, string text, bool prefixDateTime = true)
        {
            OutputPaneWriteLineAsync(serviceProvider, text, prefixDateTime).GetResultNoContext();
        }

        /// <summary>
        /// Write text with optional DateTime prefix to own output pane (create if not existing) and activate pane afterwards.
        /// </summary>
        public static async Task OutputPaneWriteLineAsync(IServiceProvider serviceProvider, string text, bool prefixDateTime = true)
        {
#if DEBUG
            Microsoft.Assumes.NotNull(serviceProvider);
#endif
            if (serviceProvider == null)
            {
                return;
            }

            if (! ThreadHelper.CheckAccess())
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            }

            var vsOutputWindow = serviceProvider.GetService<SVsOutputWindow, IVsOutputWindow>();
            if (vsOutputWindow == null)
            {
                Debug.WriteLine("Failed to get output window.");
                return;
            }

            var paneGuid = Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.GeneralPane_guid;
            // get output window or create it
            if (Microsoft.VisualStudio.ErrorHandler.Failed(vsOutputWindow.GetPane(ref paneGuid, out var extensionOutputWindow))
                || extensionOutputWindow == null)
            {
                const string paneTitle = "General";
                // the pane doesn't already exist
                if (Microsoft.VisualStudio.ErrorHandler.Failed(vsOutputWindow.CreatePane(ref paneGuid, paneTitle, Convert.ToInt32(true), Convert.ToInt32(true))))
                {
                    Debug.WriteLine("Failed to create output pane.");
                    return;
                }
                if (Microsoft.VisualStudio.ErrorHandler.Failed(vsOutputWindow.GetPane(ref paneGuid, out extensionOutputWindow))
                    || extensionOutputWindow == null)
                {
                    Debug.WriteLine("Failed to get output pane after create.");
                }
            }
            if (Microsoft.VisualStudio.ErrorHandler.Failed(extensionOutputWindow.Activate()))
            {
                Debug.WriteLine("Failed to activate output pane.");
            }

            if (prefixDateTime)
            {
                text = $"{DateTime.Now:G} {text}";
            }
            if (Microsoft.VisualStudio.ErrorHandler.Failed(extensionOutputWindow.OutputStringThreadSafe(text + Environment.NewLine)))
            {
                Debug.WriteLine("Failed to write to output pane.");
            }
        }
    }
}

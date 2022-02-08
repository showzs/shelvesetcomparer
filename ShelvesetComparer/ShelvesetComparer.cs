// <copyright file="SelectShelvesetTeamExplorerView.xaml.cs" company="http://shelvesetcomparer.codeplex.com">Copyright http://shelvesetcomparer.codeplex.com. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace WiredTechSolutions.ShelvesetComparer
{
    using System;
    using System.ComponentModel.Design;
    using System.Globalization;
    using System.Threading.Tasks;
    using Microsoft.TeamFoundation.Controls;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ShelvesetComparer
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int ShelvesetComparerResuldId = 0x0100;
        public const int ShelvesetComparerTeamExplorerViewId = 0x0200;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("b8e98565-7b6d-4d64-b51d-97fe5e56c5ec");

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
#if DEBUG
            this.OutputPaneWriteLine("Loading ..");
#endif

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
            OutputPaneWriteLine(text);
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
            if (! ThreadHelper.CheckAccess())
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            }

            var vsOutputWindow = serviceProvider.GetService<SVsOutputWindow, IVsOutputWindow>();
            if (vsOutputWindow == null)
            {
                return;
            }

            var paneGuid = new Guid(c_ExtensionOutputWindowGuid);
            // get 
            var result = vsOutputWindow.GetPane(ref paneGuid, out var extensionOutputWindow);
            if (result != Microsoft.VisualStudio.VSConstants.S_OK || extensionOutputWindow == null)
            {
                // the pane doesn't already exist
                result = vsOutputWindow.CreatePane(ref paneGuid, Resources.ToolWindowTitle, Convert.ToInt32(true), Convert.ToInt32(true));
                if (result == Microsoft.VisualStudio.VSConstants.S_OK)
                {
                    result = vsOutputWindow.GetPane(ref paneGuid, out extensionOutputWindow);
                }
            }
            if (result == Microsoft.VisualStudio.VSConstants.S_OK)
            {
                result = extensionOutputWindow.Activate();
            }

            if (prefixDateTime)
            {
                text = $"{DateTime.Now:G} {text}";
            }
            result = extensionOutputWindow.OutputStringThreadSafe(text + Environment.NewLine);
            //if (result != Microsoft.VisualStudio.VSConstants.S_OK)
            //{
            //    // TODO
            //}
        }

        // randomly generated GUID to identify the "Shelveset Comparer" output window pane
        private const string c_ExtensionOutputWindowGuid = "{38BFBA25-8AB3-4F8E-B992-930E403AA281}";
    }
}

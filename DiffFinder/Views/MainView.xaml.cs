// <copyright file="MainView.xaml.cs" company="https://github.com/rajeevboobna/CompareShelvesets">Copyright https://github.com/rajeevboobna/CompareShelvesets. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>


using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiffFinder
{
    /// <summary>
    /// The Main View of the shelveset comparison window.
    /// </summary>
    public partial class MainView : UserControl
    {
        /// <summary>
        /// The dependency property containing for the Shelveset Comparison View Model
        /// </summary>
        private static readonly DependencyProperty ComparisonModelProperty = DependencyProperty.Register("ComparisonModel", typeof(ShelvesetComparerViewModel), typeof(MainView));

        /// <summary>
        /// Keeps the visual studio version
        /// </summary>
        private static string visualStudioVersion = string.Empty;

        /// <summary>
        /// Initializes a new instance of the MainView class.
        /// </summary>
        public MainView()
        {
            this.InitializeComponent();
            this.DataContext = this;
            this.ComparisonModel = ShelvesetComparerViewModel.Instance;
        }

        /// <summary>
        /// Gets the Visual Studio Version the extension is currently running in
        /// </summary>
        public static string VisualStudioVersion
        {
            get
            {
                if (string.IsNullOrWhiteSpace(visualStudioVersion))
                {
                    visualStudioVersion = GetVisualStudioVersionAsync().GetResultNoContext();
                }

                return visualStudioVersion;
            }
        }

        /// <summary>
        /// Gets or sets the ComparisonModel
        /// </summary>
        public ShelvesetComparerViewModel ComparisonModel
        {
            get
            {
                return this.GetValue(ComparisonModelProperty) as ShelvesetComparerViewModel;
            }

            set
            {
                this.SetValue(ComparisonModelProperty, value);
            }
        }

        /// <summary>
        /// Get Visual Studio version (enforcing Main UI Thread if required)
        /// </summary>
        /// <returns></returns>
        private static async Task<string> GetVisualStudioVersionAsync()
        {
            if (! ThreadHelper.CheckAccess())
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            }

            var dte = Package.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE80.DTE2;
            return dte.SourceControl.Parent.Version;
        }

        /// <summary>
        /// The method opens up a window comparing two files
        /// </summary>
        /// <param name="compareFiles">The compare files view model</param>
        private static async System.Threading.Tasks.Task CompareFilesAsync(FileComparisonViewModel compareFiles)
        {
            string firstFileName;
            string secondFileName;
            if (compareFiles.FirstFile != null)
            {
                var extension = Path.GetExtension(compareFiles.FirstFile.FileName);
                firstFileName = string.Concat(Path.GetTempPath(), System.Guid.NewGuid().ToString(), extension);
                compareFiles.FirstFile.DownloadShelvedFile(firstFileName);
            }
            else
            {
                firstFileName = Path.GetTempFileName();
            }

            if (compareFiles.SecondFile != null)
            {
                var extension = Path.GetExtension(compareFiles.SecondFile.FileName);
                secondFileName = string.Concat(Path.GetTempPath(), System.Guid.NewGuid().ToString(), extension);
                compareFiles.SecondFile.DownloadShelvedFile(secondFileName);
            }
            else
            {
                secondFileName = Path.GetTempFileName();
            }

            if (!ThreadHelper.CheckAccess())
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            }
            var diffService = (IVsDifferenceService)Package.GetGlobalService(typeof(SVsDifferenceService));
            var firstFileDisplayName = $"{compareFiles.FirstFileDisplayName};{compareFiles.FirstShelveName}";
            var secondFileDisplayName = $"{compareFiles.SecondFileDisplayName};{compareFiles.SecondShelveName}";
            var caption = $"{compareFiles.FirstFile?.FileName ?? string.Empty} vs {compareFiles.SecondFile?.FileName ?? string.Empty}";
            var tooltip = $"{firstFileDisplayName}\r\n{secondFileDisplayName}";
            _ = diffService.OpenComparisonWindow2(firstFileName, secondFileName, caption, tooltip, firstFileDisplayName, secondFileDisplayName, null, null, 0).Show();
            File.Delete(firstFileName);
            File.Delete(secondFileName);
        }

        /// <summary>
        /// Event Handler for Mouse Double click event 
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Event Argument</param>
        private void ComparisonFiles_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e != null && e.ChangedButton == MouseButton.Left)
            {
                if (this.ComparisonFiles.SelectedItem is FileComparisonViewModel compareFiles)
                {
                    CompareFilesAsync(compareFiles).GetResultNoContext();
                }
            }
        }

        /// <summary>
        /// Event Handler for Key up event
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Event Argument</param>
        private void ComparisonFiles_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e != null && e.Key == Key.Enter)
            {
                if (this.ComparisonFiles.SelectedItem is FileComparisonViewModel compareFiles)
                {
                    CompareFilesAsync(compareFiles).GetResultNoContext();
                }
            }
        }

        /// <summary>
        /// Key up event for the search dialog
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Event Argument</param>
        private void SearchFilesTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            ShelvesetComparerViewModel.Instance.Filter = this.SearchFilesTextBox.Text;
        }
    }
}

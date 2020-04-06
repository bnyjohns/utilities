using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using FileContentSearchTool.Core.Models;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace FileContentSearchTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker worker = null;
        public MainWindow()
        {
            InitializeComponent();
            txtBoxFilePattern.Text = "*.cs";
            txtBoxFilePath.Text = @"C:\WorkSpace\depot\WebDev\Websites\SWDC\Trunk\Src";
            txtBoxSearchText.Text = "dluid";
            btnCancel.IsEnabled = false;

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void Initialize()
        {
            lblPleaseEnterPath.Foreground = Brushes.Black;
            lblPleaseFilePattern.Foreground = Brushes.Black;
            lblSearchText.Foreground = Brushes.Black;
            lblResults.Foreground = Brushes.Black;
            pbStatus.Value = 0;
            lblNumberOfFilesChanged.Content = string.Empty;
            lblResults.Text = string.Empty;            
            txtBlockFilesChanged.Text = string.Empty;           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Initialize();            
            var filePath = txtBoxFilePath.Text;
            if(string.IsNullOrEmpty(filePath))
            {
                lblPleaseEnterPath.Foreground = Brushes.Red;
                return;
            }

            var filePattern = txtBoxFilePattern.Text;
            if (string.IsNullOrEmpty(filePattern))
            {
                lblPleaseFilePattern.Foreground = Brushes.Red;
                return;
            }

            var searchText = txtBoxSearchText.Text;
            if (string.IsNullOrEmpty(searchText))
            {
                lblSearchText.Foreground = Brushes.Red;
                return;
            }

            if (!Directory.Exists(filePath))
            {
                lblResults.Foreground = Brushes.Red;
                lblResults.Text = string.Format("Path: {0}, doesnt exist!", filePath);
                return;
            }

            var replaceText = txtBoxReplaceText.Text;
            var fileSearchOption = radBtnTopDirectoryAlone.IsChecked.Value ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories;
            var contentSearchOption = radBtnSearchOnly.IsChecked.Value ? ContentSearchOption.SearchOnly : ContentSearchOption.SearchAndReplace;

            btnGo.IsEnabled = false;
            btnCancel.IsEnabled = true;
            btnBrowsePath.IsEnabled = false;

            var workerInput = new WorkerInput
            {
                FilePath = filePath,
                FilePattern = filePattern,
                ReplaceText = replaceText,
                FileSearchOption = fileSearchOption,
                SearchText = searchText,
                ContentSearchOption = contentSearchOption
            };                        
            worker.RunWorkerAsync(workerInput);            
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
            if(e.Error != null)
            {
                lblResults.Foreground = Brushes.Red;
                lblResults.Text = e.Error.ToString();
            }
            else if(e.Cancelled)
            {
                lblResults.Foreground = Brushes.Red;
                lblResults.Text = "Cancelled!";
            }
            else
            {
                lblResults.Text = "Success!!";
                lblResults.Foreground = Brushes.Green;
                var workerOutput = (WorkerOutput)e.Result;               
                txtBlockFilesChanged.Text = workerOutput.FilesChanged;
                lblNumberOfFilesChanged.Content = string.Format("No: of files matched: {0}", workerOutput.FilesChangedCount);
            }
            btnGo.IsEnabled = true;
            btnCancel.IsEnabled = false;
            btnBrowsePath.IsEnabled = true;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {            
            var workerInput = (WorkerInput)e.Argument;
            var files = Directory.GetFiles(workerInput.FilePath, workerInput.FilePattern, workerInput.FileSearchOption).ToList();
            var filesChangedCount = 0;
            var filesChanged = new StringBuilder();
            for(float i = 0; i < files.Count; i ++)
            {
                var filePath = files[(int)i];                 
                var content = File.ReadAllText(filePath, Encoding.Default);
                if (content.IndexOf(workerInput.SearchText, StringComparison.OrdinalIgnoreCase) > -1)
                {
                    if (workerInput.ContentSearchOption == ContentSearchOption.SearchAndReplace)
                    {
                        content = Regex.Replace(content, workerInput.SearchText, workerInput.ReplaceText, RegexOptions.IgnoreCase);
                        File.WriteAllText(filePath, content);
                    }
                    filesChangedCount++;
                    filesChanged.Append(filePath);
                    filesChanged.Append(Environment.NewLine);
                }
                if(worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                var percentageProgress = (int)(((i + 1) / files.Count) * 100);
                worker.ReportProgress(percentageProgress);
            }
            var workerOutput = new WorkerOutput
            {
                FilesChanged = filesChanged.ToString(),
                FilesChangedCount = filesChangedCount
            };
            e.Result = workerOutput;         
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {            
            worker.CancelAsync();
        }        

        private void btnBrowsePath_Click(object sender, RoutedEventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                var currentPath = txtBoxFilePath.Text;
                if (!string.IsNullOrEmpty(currentPath) && Directory.Exists(currentPath))
                    folderBrowserDialog.SelectedPath = currentPath;

                var result = folderBrowserDialog.ShowDialog();
                if(result == System.Windows.Forms.DialogResult.OK)
                {
                    txtBoxFilePath.Text = folderBrowserDialog.SelectedPath;
                }
            }
        }
    }
}

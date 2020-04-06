using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;


namespace ReferenceChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            txtBoxBinPath.Text = @"C:\WorkSpace\depot\WebDev\Websites\SWDC\Dev-Green\Src\Web\bin\";
            lblResults.Content = string.Empty;
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            lblResults.Content = string.Empty;
            var statusMessage = string.Empty;

            var binPath = txtBoxBinPath.Text;
            if(string.IsNullOrEmpty(binPath))
            {
                lblFilePath.Foreground = Brushes.Red;
                return;
            }

            var toSearchReferenceName = txtBoxReferenceName.Text;
            if(!Directory.Exists(binPath))
            {
                lblResults.Foreground = Brushes.Red;
                lblResults.Content = string.Format("Path: {0}, doesnt exist!", binPath);
                return;
            }

            var directoryInfo = new DirectoryInfo(binPath);
            var files = directoryInfo.GetFiles("*.exe", SearchOption.TopDirectoryOnly).Concat(directoryInfo.GetFiles("*.dll", SearchOption.TopDirectoryOnly));
            var dictionary = new Dictionary<string, List<ReferencedAssembly>>();

            if (files.Count() == 0)
                statusMessage += string.Format("No assemblies detected in the path: {0}", binPath);

            foreach (FileInfo file in files)
            {
                Assembly assembly = null;
                try
                {                    
                    assembly = Assembly.Load(File.ReadAllBytes(file.FullName));
                }
                catch
                {
                    statusMessage += string.Format("Failed to load assembly: {0}", file.FullName);
                    continue;
                }
                var referencedAssemblies = assembly.GetReferencedAssemblies();
                foreach(var referencedAssembly in referencedAssemblies)
                {
                    if(!string.IsNullOrEmpty(toSearchReferenceName) &&
                        referencedAssembly.Name.IndexOf(toSearchReferenceName, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        continue;
                    }
                    if (!dictionary.ContainsKey(referencedAssembly.Name))
                    {
                        dictionary.Add(referencedAssembly.Name, new List<ReferencedAssembly>());
                    }
                    dictionary[referencedAssembly.Name].Add(new ReferencedAssembly(referencedAssembly.Version, referencedAssembly.CodeBase, assembly));
                }
            }

            dataGrid.ItemsSource = GetDataModel(dictionary);  

            if(string.IsNullOrEmpty(statusMessage))
            {
                statusMessage = "Success!!";
                lblResults.Foreground = Brushes.Green;
            }
            else
            {
                lblResults.Foreground = Brushes.Red;
            }
            lblResults.Content = statusMessage;
        }
        
        private IEnumerable<Model> GetDataModel(Dictionary<string, List<ReferencedAssembly>> datasource)
        {
            var dataModel = new List<Model>();
            var i = 1;
            foreach(var item in datasource)
            {
                var model = new Model();
                //model.SNo = i;
                model.ReferenceAssemblyName = item.Key;
                dataModel.Add(model);
                foreach(var referenceAssembly in item.Value)
                {
                    var subModel = new Model();
                    subModel.ReferenceAssemblyVersion = referenceAssembly.VersionReferenced.ToString();
                    subModel.ReferenceAssemblyPath = referenceAssembly.CodeBase;
                    subModel.ReferencedByAssemblyName = referenceAssembly.ReferencedBy.GetName().Name;                    
                    dataModel.Add(subModel);
                }
                i++;                
            }
            return dataModel;
        }

        private void btnBrowsePath_Click(object sender, RoutedEventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                var currentPath = txtBoxBinPath.Text;
                if (!string.IsNullOrEmpty(currentPath) && Directory.Exists(currentPath))
                    folderBrowserDialog.SelectedPath = currentPath;

                var result = folderBrowserDialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    txtBoxBinPath.Text = folderBrowserDialog.SelectedPath;
                }
            }
        }

    }
}

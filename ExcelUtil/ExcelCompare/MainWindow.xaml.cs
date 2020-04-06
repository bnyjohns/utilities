using ExcelUtil.Comparer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExcelCompare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml

    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            txtBoxFilePath.Text = @"C:\Users\boney.johns\documents\Visual Studio 2012\Projects\ExcelUtil\ExcelUtil.Tests\TestExcel.xlsx";
        }

        private void Initialize()
        {
            lblPleaseEnterPath.Foreground = Brushes.Black;
            lblPleaseEnterSheetNumbers.Foreground = Brushes.Black;
            lblDifferenceCount.Foreground = Brushes.Black;
            
            lblDifferenceCount.Content = string.Empty;
            txtBlockFilesSheetDifference.Text = string.Empty;    
        }

        private void radSingleExcel_Checked(object sender, RoutedEventArgs e)
        {
            if (gridSingleExcel != null && gridTwoExcels != null)
            {
                gridSingleExcel.Visibility = Visibility.Visible;
                gridTwoExcels.Visibility = Visibility.Collapsed;
            }
        }

        private void radBtnTwoExcels_Checked(object sender, RoutedEventArgs e)
        {
            gridTwoExcels.Visibility = Visibility.Visible;
            gridSingleExcel.Visibility = Visibility.Collapsed;
        }

        private void btnCompareSheets_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnCompareSheets.IsEnabled = false;
                Initialize();

                var filePath = txtBoxFilePath.Text;
                if (string.IsNullOrEmpty(filePath))
                {
                    lblPleaseEnterPath.Foreground = Brushes.Red;
                    return;
                }

                if (!File.Exists(filePath))
                {
                    lblDifferenceCount.Foreground = Brushes.Red;
                    lblDifferenceCount.Content = string.Format("Path: {0}, doesnt exist!", filePath);
                    return;
                }

                //var fromSheets = txtBoxFromSheetNumbers.Text;
                //var toSheets = txtBoxToSheetNumbers.Text;
                //if (string.IsNullOrEmpty(fromSheets) || string.IsNullOrEmpty(toSheets))
                //{
                //    lblPleaseEnterSheetNumbers.Foreground = Brushes.Red;
                //    return;
                //}
                var fromStartSheet = (int)comboFromStartSheet.SelectedValue;
                var fromEndSheet = (int)comboFromEndSheet.SelectedValue;
                var toStartSheet = (int)comboToStartSheet.SelectedValue;
                var toEndSheet = (int)comboToEndSheet.SelectedValue;

                //var comparisonInput = new ComparisonInput
                //{
                //    ExcelA_FirstSheet_Number = fromStartSheet,
                //    ExcelA_LastSheet_Number = fromEndSheet,
                //    ExcelB_FirstSheet_Number = toStartSheet,
                //    ExcelB_LastSheet_Number = toEndSheet,
                //};

                IEnumerable<string> difference = null;
                using (var excelComparer = new ExcelSheetComparer(filePath))
                {
                    difference = excelComparer.CompareSheetsRange(fromStartSheet, fromEndSheet, toStartSheet, toEndSheet);
                }

                lblDifferenceCount.Foreground = Brushes.Green;
                lblDifferenceCount.Content = string.Format("Difference between the from and to sheets is: {0}", difference.Count());

                var result = new StringBuilder();
                difference.ToList().ForEach(d =>
                {
                    result.Append(d + Environment.NewLine);
                });

                txtBlockFilesSheetDifference.Text = result.ToString();
            }
            catch (Exception ex)
            {
                lblDifferenceCount.Foreground = Brushes.Red;
                lblDifferenceCount.Content = ex.ToString();
            }
            finally
            {
                btnCompareSheets.IsEnabled = true;
            }
        }

       

        private ComparisonInput GetComparisonInput()
        {
            throw new NotImplementedException();
        }

        private IEnumerable<int> GetComboDataSource(int min = 1, int max = 15)
        {
            return Enumerable.Range(min, max);
        }

        private void comboFromStartSheet_Loaded(object sender, RoutedEventArgs e)
        {
            comboFromStartSheet.ItemsSource = GetComboDataSource();
            comboFromStartSheet.SelectedIndex = 0;
        }

        private void comboFromEndSheet_Loaded(object sender, RoutedEventArgs e)
        {
            comboFromEndSheet.ItemsSource = GetComboDataSource();
            comboFromEndSheet.SelectedIndex = 0;
        }

        private void comboToStartSheet_Loaded(object sender, RoutedEventArgs e)
        {
            comboToStartSheet.ItemsSource = GetComboDataSource();
            comboToStartSheet.SelectedIndex = 0;
        }

        private void comboToEndSheet_Loaded(object sender, RoutedEventArgs e)
        {
            comboToEndSheet.ItemsSource = GetComboDataSource();
            comboToEndSheet.SelectedIndex = 0;
        }
        
    }
}

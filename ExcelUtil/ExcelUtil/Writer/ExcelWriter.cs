using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ExcelUtil.Writer
{
    public class ExcelWriter : IExcelWriter
    {
        private readonly Application _xlApp;
        private Workbook _xlWorkBook;
        private Worksheet _xlWorkSheet;
        private readonly string _fileName;
        private readonly object _misValue = Missing.Value;
                
        public ExcelWriter(string fileName)
        {
            _xlApp = new Application();
            if (_xlApp == null)
                throw new Exception("Excel is not properly installed!!");

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            var directory = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException(string.Format("Directory Not Found: {0}", directory));

            _fileName = fileName;
        }

        /// <summary>
        /// Writes the IEnumberable input in its string representation to the Excel Worksheet
        /// </summary>
        /// <typeparam name="T">Type of IEnumerable Input</typeparam>
        /// <param name="input">IEnumerable Input</param>
        /// <param name="workSheetNumber">WorkSheet Number</param>
        public void WriteToExcel<T>(IEnumerable<T> input, int workSheetNumber = 1)
        {
            CreateWorkSheet(workSheetNumber);
            WriteToWorkSheet<T>(input);
            SaveAndCloseWorkBook();
        }

        /// <summary>
        /// Writes the text input to the first column of the first row of the worksheet mentioned
        /// </summary>
        /// <param name="input">String Input</param>
        /// <param name="workSheetNumber">WorkSheet Number</param>
        public void WriteToExcel(string input, int workSheetNumber = 1)
        {
            WriteToExcel<string>(new List<string> { input }, workSheetNumber);
        }

        /// <summary>
        /// Splits the string input by the character array passed and then writes the split data into the multiple rows of the worksheet
        /// </summary>
        /// <param name="input">String Input to be Split</param>
        /// <param name="split">Character Split array</param>
        /// <param name="workSheetNumber">WorkSheet Number</param>
        public void WriteToExcel(string input, char[] split, int workSheetNumber = 1)
        {
            var tempInput = input.Split(split).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim());
            WriteToExcel<string>(tempInput, workSheetNumber);
        }

        private void WriteToWorkSheet<T>(IEnumerable<T> input)
        {
            var rowCount = 1;
            foreach (var item in input)
            {
                _xlWorkSheet.Cells[rowCount, 1] = item.ToString();                
                rowCount++;
            }
        }

        private void CreateWorkSheet(int workSheetNumber)
        {
            try
            {
                _xlWorkBook = _xlApp.Workbooks.Add(_misValue);
                _xlWorkSheet = (Worksheet)_xlWorkBook.Worksheets.get_Item(workSheetNumber);
            }
            catch(COMException)
            {
                throw new Exception("No workSheet exists for the worksheetnumber: " + workSheetNumber);
            }
        }

        private void SaveAndCloseWorkBook()
        {
            _xlWorkBook.SaveAs(_fileName, XlFileFormat.xlOpenXMLWorkbook, _misValue, _misValue, _misValue, _misValue,
                XlSaveAsAccessMode.xlExclusive, _misValue, _misValue, _misValue, _misValue, _misValue);
            _xlWorkBook.Close(true, _misValue, _misValue);
            _xlApp.Quit();
        }

        public void Dispose()
        {
            ReleaseObject(_xlWorkSheet);
            ReleaseObject(_xlWorkBook);
            ReleaseObject(_xlApp);
        }

        private void ReleaseObject(object obj)
        {
            try
            {
                Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch
            {
                obj = null;                
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}

using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelUtil.Reader
{
    public class ExcelReader : IExcelReader
    {
        private static readonly object LockObject = new object();
        private Application _xlApplication;
        private Workbook _xlWorkBook;

        public ExcelReader(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            Initialize(filePath);
        }

        private void Initialize(string filePath)
        {
            _xlApplication = new Application();
            if (_xlApplication == null)
            {
                throw new Exception("Excel is not properly installed!!");               
            }
            _xlWorkBook = _xlApplication.Workbooks.Open(filePath, 0, true, 5, "", "",
                true, XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
        }

        /// <summary>
        /// Returns the number of WorkSheets in the Excel
        /// </summary>
        public int WorkSheetCount
        {
            get
            {
                return _xlWorkBook.Worksheets.Count;
            }
        }

        /// <summary>
        /// Reads data from all available sheets into an IEnumerable string format
        /// </summary>
        /// <param name="rowLimit">Limits data to the rowLimit specified for the worksheet</param>
        /// <param name="columnLimit">Limits data to the columnLimit specified for the worksheet</param>
        /// <param name="maintainOrder">Does order need to be maintained for the data read from the worksheets. It is a whole lot faster if order doesnt need to be maintained as parallelism will be made use of</param>
        /// <returns>Data from the excel read</returns>
        public IEnumerable<string> ReadAllSheets(int rowLimit = 0, int columnLimit = 0, bool maintainOrder = false)
        {
            var result = new List<string>();
            try
            {
                if (!maintainOrder)
                {
                    Parallel.ForEach(_xlWorkBook.Worksheets.OfType<Worksheet>(), (workSheet) =>
                    {
                        var threadResult = ReadFromAndReleaseWorkSheet(rowLimit, columnLimit, workSheet, true);
                        lock (LockObject)
                        {
                            result.AddRange(threadResult);
                        }                        
                    });
                }
                else
                {
                    for (int i = 1; i <= WorkSheetCount; i++)
                    {
                        result.AddRange(ReadSheet(i, rowLimit, columnLimit));
                    }
                }
            }
            catch
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// Reads data from a range of sheets(increasing order) into an IEnumerable string format
        /// </summary>
        /// <param name="firstSheetNumber">First worksheet number in the range from which data needs to be read</param>
        /// <param name="lastSheetNumber">Last worksheet number in the range from which data needs to be read</param>
        /// <param name="rowLimit">Limits data to the rowLimit specified for the worksheet</param>
        /// <param name="columnLimit">Limits data to the columnLimit specified for the worksheet</param>
        /// <param name="maintainOrder">Does order need to be maintained for the data read from the worksheets. It is a whole lot faster if order doesnt need to be maintained as parallelism will be made use of</param>
        /// <returns>Data from the excel worksheets read</returns>
        public IEnumerable<string> ReadSheetsRange(int firstSheetNumber, int lastSheetNumber,
            int rowLimit = 0, int columnLimit = 0, bool maintainOrder = false)
        {
            if (firstSheetNumber < 1)
                throw new ArgumentOutOfRangeException("firstSheetNumber", "firstSheetNumber should be greater than Zero");

            if (lastSheetNumber < 1)
                throw new ArgumentOutOfRangeException("lastSheetNumber", "firstSheetNumber should be greater than Zero");

            if (lastSheetNumber > WorkSheetCount)
                throw new ArgumentOutOfRangeException("lastSheetNumber", "lastSheetNumber shouldnot be greater than the total worksheet count");

            if (firstSheetNumber > lastSheetNumber)
                throw new ArgumentException("firstSheetNumber shouldnot be greater than lastSheetNumber");

            var result = new List<string>();
            var workSheets = GetWorkSheets(firstSheetNumber, lastSheetNumber);
            if (!maintainOrder)
            {
                Parallel.ForEach(workSheets, (workSheet) =>
                {
                    var threadResult = ReadFromAndReleaseWorkSheet(rowLimit, columnLimit, workSheet, true);
                    lock (LockObject)
                    {
                        result.AddRange(threadResult);
                    }                    
                });
            }
            else
            {
                foreach (var workSheet in workSheets)
                {
                    result.AddRange(ReadFromAndReleaseWorkSheet(rowLimit, columnLimit, workSheet));
                }
            }
            return result;
        }

        private IEnumerable<Worksheet> GetWorkSheets(int firstSheetNumber, int lastSheetNumber)
        {
            for (int i = firstSheetNumber; i <= lastSheetNumber; i++)
            {
                yield return _xlWorkBook.Worksheets.get_Item(i);
            }
        }


        /// <summary>
        /// Reads data from a single sheet into an IEnumerable string format
        /// </summary>
        /// <param name="sheetNumber">Worksheet number from which data needs to be read</param>
        /// <param name="rowLimit">Limits data to the rowLimit specified for the worksheet</param>
        /// <param name="columnLimit">Limits data to the columnLimit specified for the worksheet</param>
        /// <param name="maintainOrder">Does order need to be maintained for the data read from the worksheets. It is a whole lot faster if order doesnt need to be maintained as parallelism will be made use of</param>
        /// <returns>Data from the excel read</returns>
        public IEnumerable<string> ReadSheet(int sheetNumber = 1, int rowLimit = 0, int columnLimit = 0, bool maintainOrder = false)
        {
            List<string> result = null;
            try
            {
                if (sheetNumber < 1)
                    throw new ArgumentOutOfRangeException("sheetNumber", "SheetNumber should be greater than Zero");

                var workSheet = (Worksheet)_xlWorkBook.Worksheets.get_Item(sheetNumber);
                result = ReadFromAndReleaseWorkSheet(rowLimit, columnLimit, workSheet, !maintainOrder).ToList();
            }
            catch
            {
                throw;
            }
            return result;
        }


        private IEnumerable<string> ReadFromAndReleaseWorkSheet(int rowLimit, int columnLimit,
            Worksheet xlWorkSheet, bool parallel = false)
        {
            var result = new List<string>();
            var range = xlWorkSheet.UsedRange;
            var rwLimit = rowLimit == 0 ? range.Rows.Count : rowLimit;
            var colLimit = columnLimit == 0 ? range.Columns.Count : columnLimit;

            if (parallel)
            {
                Parallel.ForEach(Enumerable.Range(1, rwLimit), rowcount =>
                {
                    for (var colCount = 1; colCount <= colLimit; colCount++)
                    {
                        var val = (range.Cells[rowcount, colCount] as Range).Value2;
                        if (val != null)
                        {
                            lock (LockObject)
                            {
                                result.Add(val.ToString());
                            }
                        }
                    }
                });
            }
            else
            {
                for (var rowcount = 1; rowcount <= rwLimit; rowcount++)
                {
                    for (var colCount = 1; colCount <= colLimit; colCount++)
                    {
                        var val = (range.Cells[rowcount, colCount] as Range).Value2;
                        if (val != null)
                        {
                            result.Add(val.ToString());
                        }
                    }
                }
            }
            ReleaseObject(xlWorkSheet);
            return result;
        }
        

        private void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
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

        public void Dispose()
        {
            _xlWorkBook.Close(true, null, null);
            _xlApplication.Quit();
            ReleaseObject(_xlWorkBook);
            ReleaseObject(_xlApplication);
        }
    }
}

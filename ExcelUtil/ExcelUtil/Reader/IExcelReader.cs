using System;
using System.Collections.Generic;

namespace ExcelUtil.Reader
{
    public interface IExcelReader : IDisposable
    {
        /// <summary>
        /// Reads data from all available sheets into an IEnumerable string format
        /// </summary>
        /// <param name="rowLimit">Limits data to the rowLimit specified for the worksheet</param>
        /// <param name="columnLimit">Limits data to the columnLimit specified for the worksheet</param>
        /// <param name="maintainOrder">Does order need to be maintained for the data read from the worksheets. It is a whole lot faster if order doesnt need to be maintained as parallelism will be made use of</param>
        /// <returns>Data from the excel read</returns>
        IEnumerable<string> ReadAllSheets(int rowLimit = 0, int columnLimit = 0, bool maintainOrder = false);
        /// <summary>
        /// Reads data from a single sheet into an IEnumerable string format
        /// </summary>
        /// <param name="sheetNumber">Worksheet number from which data needs to be read</param>
        /// <param name="rowLimit">Limits data to the rowLimit specified for the worksheet</param>
        /// <param name="columnLimit">Limits data to the columnLimit specified for the worksheet</param>
        /// <param name="maintainOrder">Does order need to be maintained for the data read from the worksheets. It is a whole lot faster if order doesnt need to be maintained as parallelism will be made use of</param>
        /// <returns>Data from the excel worksheet read</returns>
        IEnumerable<string> ReadSheet(int sheetNumber = 1, int rowLimit = 0, int columnLimit = 0, bool maintainOrder = false);
        /// <summary>
        /// Reads data from a range of sheets(increasing order) into an IEnumerable string format
        /// </summary>
        /// <param name="firstSheetNumber">First worksheet number in the range from which data needs to be read</param>
        /// <param name="lastSheetNumber">Last worksheet number in the range from which data needs to be read</param>
        /// <param name="rowLimit">Limits data to the rowLimit specified for the worksheet</param>
        /// <param name="columnLimit">Limits data to the columnLimit specified for the worksheet</param>
        /// <param name="maintainOrder">Does order need to be maintained for the data read from the worksheets. It is a whole lot faster if order doesnt need to be maintained as parallelism will be made use of</param>
        /// <returns>Data from the excel worksheets read</returns>
        IEnumerable<string> ReadSheetsRange(int firstSheetNumber, int lastSheetNumber, int rowLimit = 0, int columnLimit = 0, bool maintainOrder = false);
        /// <summary>
        /// Returns the number of WorkSheets in the Excel
        /// </summary>
        int WorkSheetCount { get; }
    }
}

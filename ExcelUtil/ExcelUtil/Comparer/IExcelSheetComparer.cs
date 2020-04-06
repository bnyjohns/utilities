using System;
using System.Collections.Generic;

namespace ExcelUtil.Comparer
{
    public interface IExcelSheetComparer : IDisposable
    {
        IEnumerable<string> CompareSheets(int sheetANumber, int sheetBNumber, int sheetARowLimit = 0, int sheetAColLimit = 0, int sheetBRowLimit = 0, int sheetBColLimit = 0);
        IEnumerable<string> CompareSheetsRange(int setAFirstSheet, int setALastSheet, int setBFirstSheet,
            int setBLastSheet, int sheetARowLimit = 0, int sheetAColLimit = 0, int sheetBRowLimit = 0, int sheetBColLimit = 0);
    }
}

using System;
using System.Collections.Generic;

namespace ExcelUtil.Comparer
{
    public interface IExcelComparer : IDisposable
    {
        IEnumerable<string> CompareExcelSheets(ComparisonInput comparisonInput, int excelARowLimit = 0, int excelAColLimit = 0, int excelBRowLimit = 0, int excelBColLimit = 0);
        IEnumerable<string> CompareExcelSheetsRange(ComparisonInput comparisonInput, int excelARowLimit = 0, int excelAColLimit = 0, int excelBRowLimit = 0, int excelBColLimit = 0);
        IEnumerable<string> CompareAllExcelSheets(int excelARowLimit = 0, int excelAColLimit = 0, int excelBRowLimit = 0, int excelBColLimit = 0);
    }
}

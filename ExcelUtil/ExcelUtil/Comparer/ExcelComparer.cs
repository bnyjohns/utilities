using ExcelUtil.Reader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExcelUtil.Comparer
{
    public class ExcelComparer : IExcelComparer
    {
        IExcelReader _excelReader1;
        IExcelReader _excelReader2;

        public ExcelComparer(string fileName1, string fileName2)
        {
            if (string.IsNullOrEmpty(fileName1))
                throw new ArgumentNullException("fileName1");

            if (!File.Exists(fileName1))
                throw new FileNotFoundException("File not found", fileName1);

            _excelReader1 = new ExcelReader(fileName1);

            if (string.IsNullOrEmpty(fileName2))
                throw new ArgumentNullException("fileName2");

            if (!File.Exists(fileName2))
                throw new FileNotFoundException("File not found", fileName2);

            _excelReader2 = new ExcelReader(fileName2);
        }

        public IEnumerable<string> CompareExcelSheets(ComparisonInput comparisonInput, int excelARowLimit = 0, int excelAColLimit = 0, int excelBRowLimit = 0, int excelBColLimit = 0)
        {
            if (comparisonInput == null)
                throw new ArgumentNullException("comparisonFactor");

            var setAItems = _excelReader1.ReadSheet(comparisonInput.ExcelA_FirstSheet_Number, excelARowLimit, excelAColLimit);
            var setBItems = _excelReader2.ReadSheet(comparisonInput.ExcelB_FirstSheet_Number, excelBRowLimit, excelBColLimit);
            return setAItems.Except(setBItems);
        }

        public IEnumerable<string> CompareExcelSheetsRange(ComparisonInput comparisonInput, int excelARowLimit = 0, int excelAColLimit = 0, int excelBRowLimit = 0, int excelBColLimit = 0)
        {
            if (comparisonInput == null)
                throw new ArgumentNullException("comparisonFactor");

            var setAItems = _excelReader1.ReadSheetsRange(comparisonInput.ExcelA_FirstSheet_Number, comparisonInput.ExcelA_LastSheet_Number, excelARowLimit, excelAColLimit);
            var setBItems = _excelReader2.ReadSheetsRange(comparisonInput.ExcelB_FirstSheet_Number, comparisonInput.ExcelB_LastSheet_Number, excelBRowLimit, excelBColLimit);
            return setAItems.Except(setBItems);            
        }

        public IEnumerable<string> CompareAllExcelSheets(int excelARowLimit = 0, int excelAColLimit = 0, int excelBRowLimit = 0, int excelBColLimit = 0)
        {
            var setAItems = _excelReader1.ReadAllSheets(excelARowLimit, excelAColLimit);
            var setBItems = _excelReader2.ReadAllSheets(excelBRowLimit, excelBColLimit);
            return setAItems.Except(setBItems);            
        }        

        public void Dispose()
        {
            if (_excelReader1 != null)
            {
                _excelReader1.Dispose();
                _excelReader1 = null;
            }

            if (_excelReader2 != null)
            {
                _excelReader2.Dispose();
                _excelReader2 = null;
            }
        }
    }
}

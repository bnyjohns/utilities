using ExcelUtil.Reader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExcelUtil.Comparer
{
    public class ExcelSheetComparer : IExcelSheetComparer
    {       
        private IExcelReader _excelReader;

        #region Constructors
        public ExcelSheetComparer(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            if (!File.Exists(fileName))
                throw new FileNotFoundException("File not found", fileName);

            _excelReader = new ExcelReader(fileName);
        }        
        #endregion        

        public IEnumerable<string> CompareSheets(int sheetANumber, int sheetBNumber, int sheetARowLimit = 0, int sheetAColLimit = 0,
            int sheetBRowLimit = 0, int sheetBColLimit = 0)
        {
            var sheetAItems = _excelReader.ReadSheet(sheetANumber, sheetARowLimit, sheetAColLimit);
            var sheetBItems = _excelReader.ReadSheet(sheetBNumber, sheetBRowLimit, sheetBColLimit);
            return sheetAItems.Except(sheetBItems); 
        }        

        public IEnumerable<string> CompareSheetsRange(int setAFirstSheet, int setALastSheet, int setBFirstSheet,
            int setBLastSheet, int sheetARowLimit = 0, int sheetAColLimit = 0, int sheetBRowLimit = 0, int sheetBColLimit = 0)
        {
            var sheetAItems = _excelReader.ReadSheetsRange(setAFirstSheet, setALastSheet, sheetARowLimit, sheetAColLimit);
            var sheetBItems = _excelReader.ReadSheetsRange(setBFirstSheet, setBLastSheet, sheetBRowLimit, sheetBColLimit);
            return sheetAItems.Except(sheetBItems);             
        }

        public void Dispose()
        {
            if (_excelReader != null)
            {
                _excelReader.Dispose();
                _excelReader = null;
            }
        }
    }
}

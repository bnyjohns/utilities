using ExcelUtil.Reader;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ExcelUtil.Tests.ReaderTests
{
    [TestClass]
    public class ExcelReaderTests : ExcelUtilTests
    {
        IExcelReader _excelReader;

        [TestInitialize] 
        public void Initialize()
        {
            var filePath = GetExcelFilePathToRead();
            _excelReader = new ExcelReader(filePath);
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (_excelReader != null)
            {
                _excelReader.Dispose();
                _excelReader = null;
            }
        }

        [TestMethod]
        public void ReadAllSheets_Test()
        {
            var result = _excelReader.ReadAllSheets();
            Assert.AreEqual(2562, result.Count());
        }

        [TestMethod]
        public void ReadSheetsRange_Test()
        {
            var result = _excelReader.ReadSheetsRange(1, 3);
            Assert.AreEqual(1462, result.Count());
        }

        [TestMethod]
        public void ReadSheet_Test()
        {
            var result = _excelReader.ReadSheet(1);
            Assert.AreEqual(648, result.Count());
        }
    }
}

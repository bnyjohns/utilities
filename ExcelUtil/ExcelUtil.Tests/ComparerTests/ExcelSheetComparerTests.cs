using ExcelUtil.Comparer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ExcelUtil.Tests.ComparerTests
{
    [TestClass]
    public class ExcelSheetComparerTests : ExcelUtilTests
    {
        IExcelSheetComparer _excelSheetComparer = null;

        [TestInitialize]
        public void Initialize()
        {
            var filePath = GetExcelFilePathToRead();
            _excelSheetComparer = new ExcelSheetComparer(filePath);
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (_excelSheetComparer != null)
            {
                _excelSheetComparer.Dispose();
                _excelSheetComparer = null;
            }
        }

        //648 - (10+19) = 619
        [TestMethod]
        public void CompareSheets_Test_1()
        {
            var result = _excelSheetComparer.CompareSheets(1, 2);
            Assert.IsTrue(result.Count() == 619);            
        }

        //559 - (19+10) = 530
        [TestMethod]
        public void CompareSheets_Test_2()
        {
            var result = _excelSheetComparer.CompareSheets(2, 1);
            Assert.IsTrue(result.Count() == 530);
        }

        //(225 + 860 except 240) = 1115
        [TestMethod]
        public void CompareSheetsRange_Test()
        {
            var result = _excelSheetComparer.CompareSheetsRange(3, 4, 5, 5);
            Assert.IsTrue(result.Count() == 1115);
        }
    }
}

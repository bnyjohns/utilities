using ExcelUtil.Comparer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;

namespace ExcelUtil.Tests.ComparerTests
{
    [TestClass]
    public class ExcelComparerTests : ExcelUtilTests
    {
        IExcelComparer _excelComparer;

        [TestInitialize]
        public void Initialize()
        {
            var filePath = GetExcelFilePathToRead();
            _excelComparer = new ExcelComparer(filePath, filePath);
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (_excelComparer != null)
            {
                _excelComparer.Dispose();
                _excelComparer = null;
            }
        }

        [TestMethod]
        public void CompareExcelSheets_Test_A()
        {
            var comparisonInput = new ComparisonInput { ExcelA_FirstSheet_Number = 1, ExcelB_FirstSheet_Number = 1 };
            var result = _excelComparer.CompareExcelSheets(comparisonInput);
            Assert.IsTrue(!result.Any());
        }

        [TestMethod]
        public void CompareExcelSheets_Test_B()
        {
            var comparisonInput = new ComparisonInput { ExcelA_FirstSheet_Number = 2, ExcelB_FirstSheet_Number = 1 };
            var result = _excelComparer.CompareExcelSheets(comparisonInput);
            Assert.IsTrue(530 == result.Count());
        }

        [TestMethod]
        public void CompareExcelSheetsRange_Test()
        {
            var comparisonInput = new ComparisonInput { ExcelA_FirstSheet_Number = 3, ExcelA_LastSheet_Number = 4,
                ExcelB_FirstSheet_Number = 5, ExcelB_LastSheet_Number = 5 };
            var result = _excelComparer.CompareExcelSheetsRange(comparisonInput);
            Assert.IsTrue(1115 == result.Count());
        }

        [TestMethod]
        public void CompareAllExcelSheets_Test()
        {
            var result = _excelComparer.CompareAllExcelSheets();
            Assert.IsTrue(!result.Any());
        }
    }
}

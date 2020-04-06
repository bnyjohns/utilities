using System.Reflection;

namespace ExcelUtil.Tests
{
    public class ExcelUtilTests
    {
        public virtual string GetExcelFilePathToRead()
        {
            var filePath = Assembly.GetExecutingAssembly().Location.Replace(@"bin\Debug\ExcelUtil.Tests.dll", "");
            filePath += "TestReadExcel.xlsx";
            return filePath;
        }

        public virtual string GetExcelFilePathToWrite()
        {
            var filePath = Assembly.GetExecutingAssembly().Location.Replace(@"bin\Debug\ExcelUtil.Tests.dll", "");
            filePath += "TestWriteExcel.xlsx";
            return filePath;
        } 
    }
}

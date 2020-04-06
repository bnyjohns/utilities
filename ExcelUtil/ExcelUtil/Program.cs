using ExcelUtil.Reader;
using ExcelUtil.Writer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            Measure(DoWorkAction);
            Console.ReadKey();
        }

        public static IEnumerable<string> DoWorkFunc()
        {
            using (var excelReader = new ExcelReader(@"C:\Temp\PerfTest.xlsx"))            
            {
                return excelReader.ReadAllSheets();
            }
        }

        public static IEnumerable<string> DoWorkAction()
        {
            using (var excelWriter = new ExcelWriter(@"C:\Temp\ExcelWrittenFile.xlsx"))
            {
                excelWriter.WriteToExcel<int>(new List<int> { 2, 3, 4, 5 });                
            }
            return null;
        }

        public static void WriteToExcel()
        {
            var data = new List<int> { 2, 3, 4, 5 };
            using (IExcelWriter excelWriter = new ExcelWriter(@"C:\Temp\ExcelWrittenFile.xlsx"))
            {
                excelWriter.WriteToExcel<int>(data, 1);
            }
        }


        public static void Measure(Func<IEnumerable<string>> handler)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var ektronURLsFromJson = handler();
            stopwatch.Stop();
            Console.WriteLine("Seconds Elapsed: {0}", stopwatch.Elapsed.TotalSeconds);
            //Console.WriteLine("URL Count: {0}", ektronURLsFromJson.Count());
        }

        public static void WriteURLsToFile(IEnumerable<string> urls, string fileNameSuffix)
        {
            var fileNameFormat = @"C:\Temp\EktronURLsFromJson-{0}.txt";
            var result = new StringBuilder();
            urls.ToList().ForEach(url =>
            {
                result.Append(url + "\n");
            });

            var fileName = string.Format(fileNameFormat, fileNameSuffix);
            TextWriter tw = new StreamWriter(fileName, false);
            tw.WriteLine(result.ToString());
            tw.Close();
        }      
    }
}

using ExcelUtil.Reader;
using ExcelUtil.Writer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExcelUtil.Tests.WriterTests
{
    [TestClass]
    public class ExcelWriterTests : ExcelUtilTests
    {
        string _filePath;
        IExcelWriter _excelWriter;
        Lazy<IExcelReader> ExcelReaderFactory { get { return new Lazy<IExcelReader>(() => new ExcelReader(_filePath)); } }
        //Making IExcelReader Lazy so that it will get created only after the file is created by the ExcelWriter
        IExcelReader ExcelReader { get { return ExcelReaderFactory.Value; } }

        [TestInitialize]
        public void Initialize()
        {
            _filePath = GetExcelFilePathToWrite();
            _excelWriter = new ExcelWriter(_filePath);            
        }


        [TestCleanup]
        public void CleanUp()
        {
            if (_excelWriter != null)
            {
                _excelWriter.Dispose();
                _excelWriter = null;
            }
            if (ExcelReader != null)
            {
                ExcelReader.Dispose();                
            }
            File.Delete(_filePath);
        }
        

        [TestMethod]
        public void WriteToExcel_IntegerArray_Test()
        {
            var writtenInput = new List<int>{ 2, 3, 4, 5 };
            _excelWriter.WriteToExcel<int>(writtenInput, 1);
            var readOutput = ExcelReader.ReadSheet(1, maintainOrder: true).
                                            Select(int.Parse).ToList();
            CollectionAssert.AreEqual(writtenInput, readOutput);
        }

        [TestMethod]
        public void WriteToExcel_SingleItem_Test()
        {
            const string writtenInput = "hello testing!";
            _excelWriter.WriteToExcel(writtenInput, 1);
            var readOutput = ExcelReader.ReadSheet(1);
            Assert.AreEqual(writtenInput, readOutput.First());
        }

        [TestMethod]
        public void WriteToExcel_StringSplit_Test()
        {
            const string writtenInput = "a\nb\nc\nd";
            _excelWriter.WriteToExcel(writtenInput, new char[] {'\n'}, 1);
            var readOutput = ExcelReader.ReadSheet(1, maintainOrder:true);
            var enumerable = readOutput as IList<string> ?? readOutput.ToList();
            Assert.AreEqual(4, enumerable.Count());
            CollectionAssert.AreEqual(new List<string> { "a", "b", "c", "d" }, enumerable.ToList());
        }
    }
}

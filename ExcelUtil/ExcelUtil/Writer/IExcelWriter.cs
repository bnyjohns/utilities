using System;
using System.Collections.Generic;

namespace ExcelUtil.Writer
{
    public interface IExcelWriter : IDisposable
    {
        /// <summary>
        /// Writes the IEnumberable input in its string representation to the Excel Worksheet
        /// </summary>
        /// <typeparam name="T">Type of IEnumerable Input</typeparam>
        /// <param name="input">IEnumerable Input</param>
        /// <param name="workSheetNumber">WorkSheet Number</param>
        void WriteToExcel<T>(IEnumerable<T> input, int workSheetNumber = 1);
        /// <summary>
        /// Writes the text input to the first column of the first row of the worksheet mentioned
        /// </summary>
        /// <param name="input">String Input</param>
        /// <param name="workSheetNumber">WorkSheet Number</param>
        void WriteToExcel(string input, int workSheetNumber = 1);
        /// <summary>
        /// Splits the string input by the character array passed and then writes the split data into the multiple rows of the worksheet
        /// </summary>
        /// <param name="input">String Input to be Split</param>
        /// <param name="split">Character Split array</param>
        /// <param name="workSheetNumber">WorkSheet Number</param>
        void WriteToExcel(string input, char[] split, int workSheetNumber = 1);
    }
}

using System.IO;

namespace FileContentSearchTool.Core.Models
{
    public class WorkerInput
    {
        public string FilePath { get; set; }
        public string FilePattern { get; set; }
        public string SearchText { get; set; }
        public string ReplaceText { get; set; }
        public SearchOption FileSearchOption { get; set; }
        public ContentSearchOption ContentSearchOption { get; set; }
    }
}

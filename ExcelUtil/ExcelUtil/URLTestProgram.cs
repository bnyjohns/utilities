using ExcelUtil.Reader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ExcelUtil
{

    class URLTestProgram
    {
        static object lockObject = new object();

        static string CleanUrl(List<string> urlList, string url)
        {   
            url = url.TrimEnd('/');
            if (!url.EndsWith(".aspx") && !url.EndsWith(".htm") && !url.EndsWith(".pdf")
                && !url.EndsWith(".html") && !url.EndsWith(".php") && !url.EndsWith(".deploy") &&
                urlList.Contains(url + ".aspx"))
            {
                url = url + ".aspx";
            }            
            return url;
        }

        //static void Main(string[] args)
        //{
        //    IEnumerable<string> urlList = null;
        //    using (var excelReader = new ExcelReader(@"C:\Users\boney.johns\AppData\Roaming\Skype\My Skype Received Files\Missing URLs-Sini from Access Logs.xlsx"))
        //    {
        //        urlList = excelReader.ReadSheet(4);
        //    }
        //    var okUrls = new List<string>();
        //    var notFoundUrls = new List<string>();
        //    var redirectUrls = new List<string>();

        //    //urlList = new List<string> { "a/", "a.aspx", "ab","c" };
        //    //urlList.AsParallel().Select(
        //    var filteredUrls = urlList.Select(s => s.Replace(".aspx/",".aspx")).ToList();
        //    filteredUrls = filteredUrls.Select(s => CleanUrl(filteredUrls, s)).ToList();
        //    filteredUrls = filteredUrls.Distinct().ToList();

        //    //Parallel.ForEach(urlList,
        //    //    new ParallelOptions { MaxDegreeOfParallelism = 180 },
        //    //    url =>
        //    //    {
        //    //        //url = "http://" + url;
        //    //        var statusCode = GetHttpResponse(url);
        //    //        lock (lockObject)
        //    //        {
        //    //            //if (statusCode == HttpStatusCode.NotFound)
        //    //            //{
        //    //            //    notFoundUrls.Add(url);
        //    //            //}
        //    //            //else
        //    //            //{
        //    //            //    okUrls.Add(url);
        //    //            //}
        //    //            if (statusCode == HttpStatusCode.OK)
        //    //            {
        //    //                okUrls.Add(url);
        //    //            }
        //    //            else if(statusCode == HttpStatusCode.NotFound)
        //    //            {
        //    //                notFoundUrls.Add(url);
        //    //            }
        //    //            else
        //    //            {
        //    //                redirectUrls.Add(url);
        //    //            }
        //    //        }
        //    //    });

        //    //Program.WriteURLsToFile(okUrls, "okUrls-08-03");
        //    //Program.WriteURLsToFile(notFoundUrls, "notFoundUrls-08-03");
        //    //Program.WriteURLsToFile(redirectUrls, "redirectUrls-08-03");

        //    Program.WriteURLsToFile(filteredUrls, "filteredUrls-08-03");
        //}

        private static HttpStatusCode GetHttpResponse(string url)
        {
            var statusCode = HttpStatusCode.OK;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentLength = 0;

                using (var webResponse = (HttpWebResponse)request.GetResponse())
                {
                    statusCode = webResponse.StatusCode;
                }
            }
            catch (WebException ex)
            {
                var resp = ex.Response as HttpWebResponse;
                if (resp != null)
                {
                    statusCode = resp.StatusCode;
                }
                //if not 404, then log
                if (ex.Status != WebExceptionStatus.ProtocolError)
                {
                    Console.WriteLine("WebException for URL: {0}. Exception Status is: {1}. Status Code is: {2}", url, ex.Status.ToString(), resp == null ? HttpStatusCode.NotFound: resp.StatusCode);
                }
                statusCode = HttpStatusCode.NotFound;
            }
            catch (Exception ex)
            {
                Console.WriteLine("BASE EXCEPTION: {0}", ex);
                statusCode = HttpStatusCode.NotFound;
            }
            return statusCode;
        }
    }

    class UrlComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            throw new NotImplementedException();
        }

        public int GetHashCode(string obj)
        {
            throw new NotImplementedException();
        }
    }
}

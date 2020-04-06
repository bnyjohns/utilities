using HtmlAgilityPack;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace SportsTrackerToStrava
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            const string workoutDownloadURL = @"https://www.sports-tracker.com/apiserver/v1/workout/exportGpx/{workoutId}?token=4p7qbvu0q66lisv261hkb36uait84fb7";
            var doc = new HtmlDocument();
            doc.Load(@"C:\Users\bjohns\Documents\Visual Studio 2015\Projects\ConsoleApplication1\ConsoleApplication1\workouts.html");

            var workouts = doc.DocumentNode.Descendants().Where(desc => desc.Name == "a");
            Console.WriteLine(workouts.Count());

            int counter = 0;
            foreach (var workout in workouts)
            {
                counter++;
                var workoutHref = workout.GetAttributeValue("href", "");
                var workoutId = workoutHref.Replace("/workout/bnyjohns/", "");
                var downloadURL = workoutDownloadURL.Replace("{workoutId}", workoutId);

                var workoutDetails = GetWorkoutDetails(downloadURL);
                Console.WriteLine($"Downloading workout ${counter}: {workoutId}");
                var fileName = GetFileName(workoutDetails);
                if (fileName != null)
                {
                    var filePath = $"C:\\Temp\\SportsTracker\\{fileName}.gpx";
                    Console.WriteLine($"Writing file ${counter}: {filePath}");
                    File.WriteAllText(filePath, workoutDetails);
                }
            }
        }
        private static string GetFileName(string workoutDetails)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(workoutDetails));
            XmlDocument document = new XmlDocument();
            document.Load(stream);
            foreach (XmlNode node in document.GetElementsByTagName("metadata"))
            {
                foreach (XmlNode node1 in node.ChildNodes)
                {
                    if (node1.Name == "name")
                        return node1.InnerText.Replace("/", "_").Replace(":", "_");
                }
            }
            return null;
        }

        private static string GetWorkoutDetails(string url)
        {
            var request = WebRequest.Create(url);
            using var response = (HttpWebResponse)request.GetResponse();
            using var dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            using StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            return reader.ReadToEnd();
        }
    }
}

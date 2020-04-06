using HtmlAgilityPack;
using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Timers;

namespace GridFrequencyChecker
{
    class Program
    {
        private static readonly string filePath = $"{Environment.CurrentDirectory}\\logs.txt";
        private const string loadDispatchCenterURL = @"https://srldc.in/";
        private static readonly System.Threading.ManualResetEvent quitEvent = new System.Threading.ManualResetEvent(false);
        private static readonly string freqLabelXpath = "/html/body/form/table/tr/td/table[1]/tr[3]/td[2]/div/div/table/tr[1]/td[2]/strong/span[2]";
        private static readonly string timeStampLabelXpath = "/html/body/form/table/tr/td/table[1]/tr[3]/td[2]/div/div/table/tr[1]/td[2]/strong/span[3]/font";
        private static Timer timer;

        static Program()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Starting the application");
            Console.WriteLine($"Frequency logs will be written to: {filePath}");
            Console.WriteLine("You can also see the logs below");
            Console.CancelKeyPress += Console_CancelKeyPress;
            ReadFrequency();
            StartTimer();
            quitEvent.WaitOne();
            StopTimer();
        }

        private static void StartTimer()
        {
            timer = new Timer(60000)
            {
                Enabled = true,
                AutoReset = true
            };
            timer.Elapsed += Timer_Elapsed;
        }

        private static void StopTimer()
        {
            timer.Stop();
            timer.Dispose();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            quitEvent.Set();
            e.Cancel = true;
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ReadFrequency();
        }

        static void ReadFrequency()
        {
            try
            {
                var doc = new HtmlDocument();
                var request = WebRequest.Create(loadDispatchCenterURL);
                request.Method = "GET";
                request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36");
                using var response = request.GetResponse();
                using var stream = response.GetResponseStream();
                using StreamReader reader = new StreamReader(stream);
                doc.Load(stream);
                var freqLabel = doc.DocumentNode.SelectSingleNode(freqLabelXpath);
                var timeStampLabel = doc.DocumentNode.SelectSingleNode(timeStampLabelXpath);
                if (freqLabel != null && timeStampLabel != null)
                {
                    var timeStamp = timeStampLabel.WriteContentTo().Replace("(", "").Replace(")", "").Trim();
                    var frequency = freqLabel.WriteContentTo();
                    var result = $"{timeStamp} : {frequency}";
                    Console.WriteLine(result);
                    File.AppendAllLines(filePath, new List<string> { result });
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.ToString()}");
            }
        }
    }
}

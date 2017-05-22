using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Pastebin_scraper
{
    class Program
    {
        private static readonly string RawUrl = "https://pastebin.com/raw/";
        private static string ArchiveUrl = "https://pastebin.com/Qnxrvy4h";
        private static List<string> Scraped = new List<string>();

        static void Main(string[] args)
        {
            if (!Directory.Exists("Pastes"))
                Directory.CreateDirectory("Pastes");
            while (true)
            {
                string Html = GetHtml(ArchiveUrl);
                Regex ItemRegex = new Regex(@"<li><a href=""/(.*?)[a-zA-Z0-9]{8}"">", RegexOptions.Compiled);
                foreach (Match ItemMatch in ItemRegex.Matches(Html))
                {
                    string Match = ItemMatch.ToString().Replace(@"<li><a href=""/", string.Empty).Replace(@""">", string.Empty);
                    if (Scraped.Contains(Match))
                        continue;
                    Console.WriteLine("Paste: " + Match);
                    ArchiveUrl = "http://pastebin.com/" + Match;
                    Thread ScrapeThread = new Thread(() => Scrape(Match));
                    ScrapeThread.IsBackground = true;
                    ScrapeThread.Start();
                    Scraped.Add(Match);
                }
            }
        }
  
        private static void Scrape(string Id)
        {
            try
            {
                string Html = GetHtml(RawUrl + Id);
                TextWriter tWriter = new StreamWriter("Pastes/" + Id + ".txt");
                tWriter.WriteLine(Html);
                tWriter.Close();
            }
            catch { }
        }
        private static string GetHtml(string Url)
        {
            using (WebClient Client = new WebClient())
                return Client.DownloadString(Url);
        }
    }
}
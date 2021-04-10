using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Net.Http;
using System.IO;

namespace k173613_Q1
{
    class DownloadPSXFile
    {
        private readonly Timer _timer;
        string file_path = ConfigurationManager.AppSettings.Get("FilePath");        // Where to Save when Download
        string file_url = ConfigurationManager.AppSettings.Get("PSX-URL-Path");     // Link from where to Download

        public DownloadPSXFile()
        {
            _timer = new Timer(1000 * 60 * 5) { AutoReset = true };     // Every Five Minute
            _timer.Elapsed += _timer_Elapsed;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            String url = file_url;
            String path = file_path;

            var httpClient = new HttpClient();
            var html = httpClient.GetStringAsync(url);

            int day = DateTime.Today.Day;
            int month = DateTime.Today.Month;
            int year = DateTime.Today.Year;

            DateTime date = new DateTime(year, month, day);
            //string file_name = @path + "/Summary" + date.ToString("dd") + date.ToString("MMM") + date.ToString("yy") + ".html";
            string file_name = @path + "/Summary-PSX-k173613-Q1.html";

            string content = html.Result;
            File.WriteAllText(file_name, content);
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

    }
}

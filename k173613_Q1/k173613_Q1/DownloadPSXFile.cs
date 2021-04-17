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

        public DownloadPSXFile()
        {
            _timer = new Timer(1000 * 60 * 5) { AutoReset = true };     // Every Five Minute
            _timer.Elapsed += _timer_Elapsed;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string path = ConfigurationManager.AppSettings.Get("FilePath");        // Where to Save when Download
            string url = ConfigurationManager.AppSettings.Get("PSX-URL-Path");     // Link from where to Download

            try
            {
                var httpClient = new HttpClient();
                var html = httpClient.GetStringAsync(url);

                string file_name = @path;   // D:/Summary-PSX-k173613-Q1.html in AppConfig

                string content = html.Result;
                File.WriteAllText(file_name, content);
            }
            catch(Exception ee)
            {
                // Generate Log File
                string log_file = ConfigurationManager.AppSettings.Get("Log-File");

                // Date Time Error Occured
                DateTime logDT = DateTime.Now;
                File.AppendAllLines(log_file, new[] { logDT.ToString() + " : DownloadPSXFile - Error Occured due to File Not Found OR Internet Issue...!" });
            }
            
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

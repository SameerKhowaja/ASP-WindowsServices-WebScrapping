using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace k173613_Q1
{
    class Program
    {
        static void Main(string[] args)
        {
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<DownloadPSXFile>(s =>
                {
                    s.ConstructUsing(downloadPSXFile => new DownloadPSXFile());
                    s.WhenStarted(downloadPSXFile => downloadPSXFile.Start());
                    s.WhenStopped(downloadPSXFile => downloadPSXFile.Stop());
                });
                x.RunAsLocalSystem();
                x.SetServiceName("k173613_Q1");
                x.SetDisplayName("Downloading PSX Web Page Periodically By K173613_Q1");
                x.SetDescription("This service will download PSX web page as html file every 5 minutes. Developed By Sameer Khowaja K173613_Q1.");
            });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}

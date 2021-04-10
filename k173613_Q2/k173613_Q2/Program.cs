using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace k173613_Q2
{
    class Program
    {
        static void Main(string[] args)
        {
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<ParsingPSXData>(s =>
                {
                    s.ConstructUsing(parsingPSXData => new ParsingPSXData());
                    s.WhenStarted(parsingPSXData => parsingPSXData.Start());
                    s.WhenStopped(parsingPSXData => parsingPSXData.Stop());
                });
                x.RunAsLocalSystem();
                x.SetServiceName("ParsingPSXData");
                x.SetDisplayName("Parsing PSX Data By K173613_Q2");
                x.SetDescription("This service will Parse data of downloaded Html file and create XML for all Category in seperate folder every 10 minutes. Developed By Sameer Khowaja K173613_Q2.");
            });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}

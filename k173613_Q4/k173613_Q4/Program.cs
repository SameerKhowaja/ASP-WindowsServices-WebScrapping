using System;
using Topshelf;

namespace k173613_Q4
{
    class Program
    {
        static void Main(string[] args)
        {
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<HistoricalData>(s =>
                {
                    s.ConstructUsing(historicalData => new HistoricalData());
                    s.WhenStarted(historicalData => historicalData.Start());
                    s.WhenStopped(historicalData => historicalData.Stop());
                });
                x.RunAsLocalSystem();
                x.SetServiceName("k173613_Q4");
                x.SetDisplayName("Historical PSX Data By K173613_Q3");
                x.SetDescription("This service is responsible to combine XML files and generate a JSON file for each of the script every 20 minutes. If the JSON file does not exist, it will create a new file but if the file already exists, it will append to the same file.The field lastUpdatedOn will be changed whenever the file has been last modified.Once the service has read the XML file and generated a JSON file, it will delete the XML file. Developed By Sameer Khowaja K173613_Q3.");
            });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}

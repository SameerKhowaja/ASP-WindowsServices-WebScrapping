using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace k173613_Q3
{
    class Program
    {
        static void Main(string[] args)
        {
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<AchievingFiles>(s =>
                {
                    s.ConstructUsing(achievingFiles => new AchievingFiles());
                    s.WhenStarted(achievingFiles => achievingFiles.Start());
                    s.WhenStopped(achievingFiles => achievingFiles.Stop());
                });
                x.RunAsLocalSystem();
                x.SetServiceName("k173613_Q3");
                x.SetDisplayName("Achieving PSX Files Data By K173613_Q3");
                x.SetDescription("This service is responsible to move all the old XML files except for the most recent one and store to another location every 15 minutes. Developed By Sameer Khowaja K173613_Q3.");
            });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}

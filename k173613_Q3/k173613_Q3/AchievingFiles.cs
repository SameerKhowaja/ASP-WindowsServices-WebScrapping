using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace k173613_Q3
{
    class AchievingFiles
    {
        private readonly Timer _timer;

        public AchievingFiles()
        {
            _timer = new Timer(1000 * 60 * 15) { AutoReset = true };     // Every 15 Minutes
            _timer.Elapsed += _timer_Elapsed;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string Old_Dir_Path = ConfigurationManager.AppSettings.Get("FreshRecordsPath");   // Old Dir Path from where to get folders
            string New_Dir_Path = ConfigurationManager.AppSettings.Get("PastRecordsPath");   // New Dir Path you want to save old folders

            // Generate Folders
            System.IO.Directory.CreateDirectory(New_Dir_Path);

            try
            {
                // Get Lastly Created/Modified Folder Path
                var directory = new DirectoryInfo(Old_Dir_Path);
                var myFolder = (from f in directory.GetDirectories()
                                orderby f.LastWriteTime descending
                                select f).First();
                String myFolder_str = Convert.ToString(myFolder);   // Latest Folder Name
                                                                    // Console.WriteLine(myFolder);

                // Get All folders names
                var Folders = new DirectoryInfo(Old_Dir_Path).GetDirectories().Select(x => x.Name).ToArray();
                // Console.WriteLine(Folders.Length);

                if (Folders.Length > 1)
                {
                    foreach (String folder_name in Folders)
                    {
                        if (folder_name != myFolder_str)     // Except Recent Folder
                        {
                            try
                            {
                                Directory.Move(Old_Dir_Path + folder_name, New_Dir_Path + folder_name);
                            }
                            catch (IOException exp)
                            {
                                Console.WriteLine(exp.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                // Something wrong happen
                // Generate Log File
                string log_file = ConfigurationManager.AppSettings.Get("Log-File");

                // Date Time Error Occured
                DateTime logDT = DateTime.Now;
                File.AppendAllLines(log_file, new[] { logDT.ToString() + " : AchievingFiles - Error Occured due No Records Found...!" });
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

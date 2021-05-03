using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml;
using System.Timers;

namespace k173613_Q4
{
    public class Data
    {
        public string Date { get; set; }
        public float Price { get; set; }
    }

    public class LastUpdatedOn
    {
        public string lastUpdatedOn { get; set; }
        public List<Data> Data { get; set; }
    }

    public class ScriptData
    {
        public LastUpdatedOn scriptData { get; set; }
    }

    class HistoricalData
    {
        private readonly Timer _timer;

        public HistoricalData()
        {
            _timer = new Timer(1000 * 60 * 20) { AutoReset = true };     // Every 20 Minutes
            _timer.Elapsed += _timer_Elapsed;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string Folder_path_xml = ConfigurationManager.AppSettings.Get("RecordsPathXML");
            string Folder_path_json = ConfigurationManager.AppSettings.Get("RecordsPathJSON");

            // Generate Folders for JSON Files
            System.IO.Directory.CreateDirectory(Folder_path_json);

            try
            {
                // Get All folders names
                var myFolder_str = new DirectoryInfo(Folder_path_xml).GetDirectories().Select(x => x.Name).ToArray();

                // For Every Folders i.e. 1742021193848, 1842021193848, ... etc
                foreach (string myFolder in myFolder_str)
                {
                    // Local System Current Date Time
                    DateTime currentDateTime = DateTime.Now;
                    String date = currentDateTime.Day.ToString() + currentDateTime.Month.ToString() + currentDateTime.Year.ToString();  // Date
                    String time = currentDateTime.Hour.ToString() + currentDateTime.Minute.ToString() + currentDateTime.Second.ToString();  // Time
                    String localSystem_dateTime = date + time;     // Date Time like 1142021153212

                    // Get All folders names inside main folder (This contain Category name)
                    var Folders = new DirectoryInfo(Folder_path_xml + myFolder).GetDirectories().Select(x => x.Name).ToArray();

                    // XML Parser
                    XmlDocument doc = new XmlDocument();

                    //Console.WriteLine(myFolder);

                    // Parsing each Folder contain  XML File
                    foreach (String folder in Folders)
                    {
                        //Console.WriteLine(folder);

                        // Generate Folders in JSON Folder of Category Names
                        System.IO.Directory.CreateDirectory(Folder_path_json + folder);

                        // Folder XML
                        string[] Files = Directory.GetFiles(Folder_path_xml + myFolder + @"\" + folder, "*.xml");
                        // Parsing each XML file and make JSON
                        foreach (String file_name in Files)
                        {
                            //Console.WriteLine(file_name);

                            // Get the creation time of a well-known directory.
                            DateTime creation_dt = Directory.GetLastWriteTime(file_name);
                            String Fol_date = creation_dt.Day.ToString() + creation_dt.Month.ToString() + creation_dt.Year.ToString();  // Date
                            String Fol_time = creation_dt.Hour.ToString() + creation_dt.Minute.ToString() + creation_dt.Second.ToString();  // Time
                            String File_dateTime = Fol_date + Fol_time;     // Date Time like 1142021153212
                            //Console.WriteLine(File_dateTime);

                            // Load XML Content
                            doc.Load(file_name);
                            XmlNodeList node = doc.GetElementsByTagName("Price");
                            String scriptPrize = node[0].InnerText;

                            // File name without and path extension
                            String FileName = Path.GetFileNameWithoutExtension(file_name);

                            // Create and Load JSON Data if JSON File Not Exist
                            if (File.Exists(Folder_path_json + folder + "\\" + FileName + ".json"))
                            {
                                // Append Data if JSON File Already Exists
                                var filePath = Folder_path_json + folder + "\\" + FileName + ".json";

                                // Read existing json data as Text
                                var jsonData = System.IO.File.ReadAllText(filePath);

                                // DeserializeObject JSON
                                dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonData);

                                // Update lastUpdatedOn 
                                jsonObject[0]["scriptData"]["lastUpdatedOn"] = localSystem_dateTime;

                                // Append Date and Price to Data
                                string json = "{\"Date\": \"" + File_dateTime + "\", \"Price\": " + scriptPrize + " }";
                                JObject rss = JObject.Parse(json);
                                jsonObject[0]["scriptData"]["Data"].Add(rss);

                                // SerializeObject JSON and Write to File
                                string outputJSON = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject, Newtonsoft.Json.Formatting.Indented);
                                System.IO.File.WriteAllText(Folder_path_json + folder + "\\" + FileName + ".json", outputJSON);
                                // Console.WriteLine(outputJSON);
                            }
                            else
                            {
                                // Creating JSON First Time if not Exists
                                List<ScriptData> _data = new List<ScriptData>();
                                _data.Add(new ScriptData()
                                {
                                    scriptData = new LastUpdatedOn()
                                    {
                                        lastUpdatedOn = localSystem_dateTime,   // Date Time like 1142021153212
                                        Data = new List<Data>() {
                                            new Data() {
                                                Date = File_dateTime,
                                                Price = float.Parse(scriptPrize)
                                            }
                                        }
                                    }
                                });

                                // Serialize and Write to File
                                string json = Newtonsoft.Json.JsonConvert.SerializeObject(_data, Newtonsoft.Json.Formatting.Indented);
                                System.IO.File.WriteAllText(Folder_path_json + folder + "\\" + FileName + ".json", json);
                                // Console.WriteLine(json);
                            }
                        }
                    }

                    // Delete Folder Directory after Parsed
                    Directory.Delete(Folder_path_xml + myFolder, true);
                }
            }
            catch(Exception ee)
            {
                // Something get wrong
                // Generate Log File
                string log_file = ConfigurationManager.AppSettings.Get("Log-File");

                // Date Time Error Occured
                DateTime logDT = DateTime.Now;
                File.AppendAllLines(log_file, new[] { logDT.ToString() + " : HistoricalData - Error Occured due to NO NEW Records Found...!" });
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

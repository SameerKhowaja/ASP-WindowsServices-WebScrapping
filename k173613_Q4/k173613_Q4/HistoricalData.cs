﻿using Newtonsoft.Json;
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
                // Get Lastly Created/Modified Folder Path
                var directory = new DirectoryInfo(Folder_path_xml);
                // Console.WriteLine(directory);
                var myFolder = (from f in directory.GetDirectories()
                                orderby f.LastWriteTime descending
                                select f).First();
                String myFolder_str = Convert.ToString(myFolder);   // Latest Folder Name

                // Get All folders names
                var Folders = new DirectoryInfo(Folder_path_xml + myFolder_str).GetDirectories().Select(x => x.Name).ToArray();

                // Local System Current Date Time
                DateTime currentDateTime = DateTime.Now;
                String date = currentDateTime.Day.ToString() + currentDateTime.Month.ToString() + currentDateTime.Year.ToString();  // Date
                String time = currentDateTime.Hour.ToString() + currentDateTime.Minute.ToString() + currentDateTime.Second.ToString();  // Time
                String dateTime = date + time;     // Date Time like 1142021153212
                                                   // Console.WriteLine(dateTime);

                // XML Parser
                XmlDocument doc = new XmlDocument();

                // Parsing each Folder contain  XML File
                foreach (String folder in Folders)
                {
                    // Folder XML
                    string[] Files = Directory.GetFiles(Folder_path_xml + myFolder_str + @"\" + folder, "*.xml");
                    // Generate Folders in JSON Folder of Category Names
                    System.IO.Directory.CreateDirectory(Folder_path_json + folder);

                    // Parsing each XML file and make JSON
                    foreach (String file_name in Files)
                    {
                        // Load XML Content
                        doc.Load(file_name);
                        XmlNodeList node1 = doc.GetElementsByTagName("Script");
                        XmlNodeList node2 = doc.GetElementsByTagName("Price");
                        String scriptName = node1[0].InnerText;
                        String scriptPrize = node2[0].InnerText;

                        // File name without and path extension
                        String FileName = Path.GetFileNameWithoutExtension(file_name);
                        // Console.WriteLine(FileName);

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
                            jsonObject[0]["scriptData"]["lastUpdatedOn"] = dateTime;

                            // Append Date and Price to Data
                            string json = "{\"Date\": \"" + dateTime.ToString() + "\", \"Price\": " + scriptPrize + " }";
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
                                    lastUpdatedOn = dateTime,   // Date Time like 1142021153212
                                    Data = new List<Data>() {
                                    new Data() {
                                        Date = dateTime,
                                        Price = float.Parse(scriptPrize)
                                    }
                                }
                                }
                            }
                            );
                            // Serialize and Write to File
                            string json = Newtonsoft.Json.JsonConvert.SerializeObject(_data, Newtonsoft.Json.Formatting.Indented);
                            System.IO.File.WriteAllText(Folder_path_json + folder + "\\" + FileName + ".json", json);
                            // Console.WriteLine(json);
                        }

                    }
                }

                // Delete XML Folder Directory
                Directory.Delete(Folder_path_xml + myFolder_str, true);
            }
            catch(Exception ee)
            {
                // Something get wrong
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

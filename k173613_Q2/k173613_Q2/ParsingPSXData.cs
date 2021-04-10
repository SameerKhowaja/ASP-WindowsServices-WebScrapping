using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;

namespace k173613_Q2
{
    class ParsingPSXData
    {
        private readonly Timer _timer;

        public ParsingPSXData()
        {
            _timer = new Timer(1000 * 60 * 2) { AutoReset = true };     // Every 10 Minutes
            _timer.Elapsed += _timer_Elapsed;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string file_name = "";
            file_name = ConfigurationManager.AppSettings.Get("FilePath");        // HTML PSX File Path

            try
            {
                string check = File.ReadAllText(file_name);
            }
            catch (Exception b)
            {
                // Console.WriteLine("File not found...!");
                int code = 0;
                Environment.Exit(code);
            }

            string htmlFile = File.ReadAllText(file_name);
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlFile);

            int totalCategory = doc.DocumentNode.SelectNodes("//div[@class='table-responsive']").Count;
            // Console.WriteLine("Total Categories: " + totalCategory);

            String[] categories = new String[totalCategory];
            int i = 0;
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div[@class='table-responsive']"))
            {
                categories[i] = node.SelectSingleNode(".//h4").InnerHtml.Trim().Replace("/", "-");   // Category Heading
                i += 1;
            }

            // Dir Path Config
            string dir_path = "";
            dir_path = ConfigurationManager.AppSettings.Get("DirPath");

            // Generate Folders
            System.IO.Directory.CreateDirectory(dir_path);
            DateTime currentDateTime = DateTime.Now;
            String mainFolder = dir_path + Convert.ToString(currentDateTime).Replace(":", ".");
            System.IO.Directory.CreateDirectory(mainFolder);    // main folder
            for (int j = 0; j < totalCategory; j++)
            {
                System.IO.Directory.CreateDirectory(@mainFolder + "/" + categories[j]);     // sub categories folders
            }

            // Time to scrap main data
            i = 0;
            int count;
            int[] category_wise_count = new int[totalCategory];
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div[@class='table-responsive']"))
            {
                count = 0;
                foreach (HtmlNode subnode in node.Descendants("tr"))
                {
                    count += 1;
                }
                count -= 2;
                category_wise_count[i] = count;
                i += 1;
            }

            int totalScripts = 0;
            for (int j = 0; j < category_wise_count.Length; j++)
            {
                totalScripts += category_wise_count[j];
            }

            List<String> scriptsName = new List<String>();
            List<String> currentPrize = new List<String>();
            String value;
            foreach (HtmlNode subnode in doc.DocumentNode.SelectNodes("//div[@class='table-responsive']//tr//td[1]"))
            {
                value = subnode.InnerText.Trim();
                if (value != "SCRIP")
                {
                    //Console.WriteLine(value);
                    scriptsName.Add(value);
                }
            }

            foreach (HtmlNode subnode in doc.DocumentNode.SelectNodes("//div[@class='table-responsive']//tr//td[6]"))
            {
                value = subnode.InnerText.Trim();
                if (value != "CURRENT")
                {
                    //Console.WriteLine(value);
                    currentPrize.Add(value);
                }
            }

            String[] scriptsNameArray = scriptsName.ToArray();
            String[] currentPrizeArray = currentPrize.ToArray();

            /*
            for(int j=0; j<currentPrizeArray.Length; j++)
            {
                Console.WriteLine("{0} --- {1}", scriptsNameArray[j], currentPrizeArray[j]);
            }
            */

            //Console.WriteLine(totalScripts);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            int kloop = 0;
            String categoryPath = mainFolder;
            for (int j = 0; j < totalCategory; j++)
            {
                String CategoryName = categories[j];
                // Console.WriteLine(CategoryName);
                for (int k = 0; k < category_wise_count[j]; k++)
                {
                    XmlWriter writer = XmlWriter.Create(categoryPath + "/" + CategoryName + "/" + scriptsNameArray[kloop] + ".xml", settings);
                    writer.WriteStartDocument();

                    writer.WriteStartElement("xml");
                    writer.WriteStartElement("Scripts");
                    writer.WriteElementString("Script", scriptsNameArray[kloop]);
                    writer.WriteElementString("Price", currentPrizeArray[kloop]);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Flush();
                    writer.Close();

                    // Console.WriteLine("{0} --- {1}", scriptsNameArray[kloop], currentPrizeArray[kloop]);
                    kloop++;
                }
            }
            // End
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

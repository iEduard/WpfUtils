using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;


namespace WpfAppLib.MultiUpdater
{
    /// <summary>
    /// Structure for the application settings to update the applications
    /// </summary>
    public struct updaterSettingsData
    {

        public string appName { get; set; }
        public string appFileName { get; set; }
        public string appServerPath { get; set; }
        public string appLocalPath { get; set; }

        public string settingsFileName { get; set; }
        public string settingsServerPath { get; set; }
        public string settingsLocalPath { get; set; }

    }

    /// <summary>
    /// Class to load the updater application settings
    /// </summary>
    static class UpdaterSettings
    {

        /// <summary>
        /// Load the settings and return the params
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<updaterSettingsData> load(string filePath, string localPath)
        {
            List<updaterSettingsData> settingsData = new List<updaterSettingsData>();

            // Try to read the Settings File
            XmlDocument doc = new XmlDocument();// Create a new XmlDocumentObject

            // Try to find the Settingsfile
            if (!File.Exists(filePath))
            {
                Console.WriteLine("No Settingsfile found. Please create settings file first and restart the application");
            }
            else
            {
                try
                {
                    Console.WriteLine("Start doc load");
                    // open the XML File
                    doc.Load(filePath);//Load the XML Data

                    Console.WriteLine("get the node list");

                    // Read 
                    XmlNodeList nodeList = doc.DocumentElement.SelectNodes("app");

                    Console.WriteLine("Get the app entry elements from the node list");
                    foreach (XmlNode appEntry in nodeList)
                    {
                        //appEntry.SelectSingleNode
                        updaterSettingsData appData = new updaterSettingsData();

                        appData.appName = appEntry.SelectSingleNode("name").InnerText;
                        appData.appFileName = appEntry.SelectSingleNode("appFileName").InnerText;
                        appData.appLocalPath = localPath + @"\" + appEntry.SelectSingleNode("appLocalPath").InnerText;
                        appData.appServerPath = appEntry.SelectSingleNode("appServerPath").InnerText;
                        appData.settingsFileName = appEntry.SelectSingleNode("settingsFileName").InnerText;
                        appData.settingsLocalPath = localPath + @"\" + appEntry.SelectSingleNode("settingsLocalPath").InnerText;
                        appData.settingsServerPath = appEntry.SelectSingleNode("settingsServerPath").InnerText;
                        settingsData.Add(appData);
                    }



                }
                catch (Exception err)
                {
                    Console.WriteLine("Error Occured during reading of the Settings file. Path: " + filePath + "\n\nError: " + err);
                }

            }

            return settingsData;
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Xml;
using CouchPotato.AppMenuItems;

namespace CouchPotato
{
    public static class ConfigFileParser
    {
        private static XmlReader ReadConfigFile()
        {
            XmlReader reader = null;
            try
            {
                reader = XmlReader.Create("Config.xml");
            }
            catch (FileNotFoundException)
            {
                WriteDefaultConfigFile("Config.xml");
                reader = XmlReader.Create("Config.xml");
            }
            return reader;
        }

        public static string LoadBrowserPath()
        {
            XmlReader reader = ReadConfigFile();
            reader.ReadToNextSibling("configuration");
            reader.ReadToDescendant("options");
            reader.ReadToDescendant("browser");
            reader.ReadToDescendant("path");
            return reader.ReadElementContentAsString();
        }

        public static string LoadBrowserProcessName()
        {
            XmlReader reader = ReadConfigFile();
            reader.ReadToNextSibling("configuration");
            reader.ReadToDescendant("options");
            reader.ReadToDescendant("browser");
            reader.ReadToDescendant("processName");
            return reader.ReadElementContentAsString();
        }

        public static int LoadWidth()
        {
            XmlReader reader = ReadConfigFile();
            reader.ReadToNextSibling("configuration");
            reader.ReadToDescendant("options");
            reader.ReadToDescendant("width");
            return reader.ReadElementContentAsInt();
        }

        public static List<AppMenuItem> LoadMenuItems()
        {
            List<AppMenuItem> menuItems = new List<AppMenuItem>();
            XmlReader reader = ReadConfigFile();
            reader.ReadToNextSibling("configuration");
            reader.ReadToDescendant("menuItems");
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name.Equals("website"))
                    {
                        menuItems.Add(ParseWebsiteItem(reader.ReadSubtree()));
                    }
                    else if (reader.Name.Equals("program"))
                    {
                        menuItems.Add(ParseProgramItem(reader.ReadSubtree()));
                    }
                    else if (reader.Name.Equals("controllerProgram"))
                    {
                        menuItems.Add(ParseControllerProgramItem(reader.ReadSubtree()));
                    }
                }
            }
            return menuItems;
        }

        public static WebsiteItem ParseWebsiteItem(XmlReader reader)
        {
            string name = null;
            string url = null;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name.Equals("name"))
                    {
                        name = reader.ReadElementContentAsString();
                    }
                    else if (reader.Name.Equals("url"))
                    {
                        url = reader.ReadElementContentAsString();
                    }
                }
            }
            return new WebsiteItem(name, url);
        }

        public static ProgramItem ParseProgramItem(XmlReader reader)
        {
            Dictionary<string, string> members = ParseProgramItemHelper(reader);
            return new ProgramItem(members["name"], members["processName"], members["processPath"], members["args"], members["appStartedArgs"]);
        }

        public static ControllerProgramItem ParseControllerProgramItem(XmlReader reader)
        {
            Dictionary<string, string> members = ParseProgramItemHelper(reader);
            return new ControllerProgramItem(members["name"], members["processName"], members["processPath"], members["args"], members["appStartedArgs"]);
        }

        public static Dictionary<string, string> ParseProgramItemHelper(XmlReader reader)
        {
            string name = null;
            string processName = null;
            string processPath = null;
            string args = null;
            string appStartedArgs = null;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name.Equals("name"))
                    {
                        name = reader.ReadElementContentAsString();
                    }
                    else if (reader.Name.Equals("processName"))
                    {
                        processName = reader.ReadElementContentAsString();
                    }
                    else if (reader.Name.Equals("processPath"))
                    {
                        processPath = reader.ReadElementContentAsString();
                    }
                    else if (reader.Name.Equals("args"))
                    {
                        args = reader.ReadElementContentAsString();
                    }
                    else if (reader.Name.Equals("appStartedArgs"))
                    {
                        appStartedArgs = reader.ReadElementContentAsString();
                    }
                }
            }
            return new Dictionary<string, string>()
            {
                { "name", name },
                { "processName", processName },
                { "processPath", processPath },
                { "args", args },
                { "appStartedArgs", appStartedArgs }
            };
        }

        private static void WriteDefaultConfigFile(string fileName)
        {
            System.Diagnostics.Debug.WriteLine("Writing default config file");
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(fileName, settings);
            writer.WriteStartElement("configuration");
            writer.WriteStartElement("menuItems");
            WriteWebsiteEntry(writer, "Netflix", "https://www.netflix.com");
            WriteWebsiteEntry(writer, "Hulu", "https://www.hulu.com");
            WriteWebsiteEntry(writer, "Amazon Video", "https://www.amazon.com/video");
            WriteWebsiteEntry(writer, "HBO Now", "https://play.hbonow.com/");
            WriteProgramEntry(writer, "Steam Big Picture", "Steam", "C:\\Program Files (x86)\\Steam\\steam.exe", "-bigPicture", "steam://open/bigpicture", true);
            writer.WriteEndElement();
            writer.WriteStartElement("options");
            writer.WriteStartElement("browser");
            writer.WriteStartElement("processName");
            writer.WriteString("Chrome");
            writer.WriteEndElement();
            writer.WriteStartElement("path");
            writer.WriteString(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("width");
            writer.WriteString("960");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Close();
        }

        private static void WriteWebsiteEntry(XmlWriter writer, string name, string url)
        {
            writer.WriteStartElement("website");
            writer.WriteStartElement("name");
            writer.WriteString(name);
            writer.WriteEndElement();
            writer.WriteStartElement("url");
            writer.WriteString(url);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private static void WriteProgramEntry(XmlWriter writer, string name, string processName,
            string processPath, string args, string appStartedArgs, bool controllerSupported)
        {
            string programElementName = controllerSupported ? "controllerProgram" : "program";
            writer.WriteStartElement(programElementName);
            writer.WriteStartElement("name");
            writer.WriteString(name);
            writer.WriteEndElement();
            writer.WriteStartElement("processName");
            writer.WriteString(processName);
            writer.WriteEndElement();
            writer.WriteStartElement("processPath");
            writer.WriteString(processPath);
            writer.WriteEndElement();
            writer.WriteStartElement("args");
            writer.WriteString(args);
            writer.WriteEndElement();

            if (appStartedArgs != null)
            {
                writer.WriteStartElement("appStartedArgs");
                writer.WriteString(appStartedArgs);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }
}

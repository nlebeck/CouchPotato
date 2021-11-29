using System.Collections.Generic;
using System.IO;
using System.Xml;
using CouchPotato.AppMenuItems;
using CouchPotato.ButtonMappings;

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
            ButtonMapping buttonMapping = new DefaultButtonMapping();
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
                    else if (reader.Name.Equals("buttonMapping"))
                    {
                        buttonMapping = ParseButtonMapping(reader.ReadSubtree());
                    }
                }
            }
            return new WebsiteItem(name, url, buttonMapping);
        }

        public static ButtonMapping ParseButtonMapping(XmlReader reader)
        {
            string keyForGamepadBack = null;
            string keyForGamepadStart = null;
            string keyForGamepadDpadLeft = null;
            string keyForGamepadDpadRight = null;
            string keyForGamepadDpadDown = null;
            string keyForGamepadDpadUp = null;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "back")
                    {
                        keyForGamepadBack = reader.ReadElementContentAsString();
                    }
                    else if (reader.Name == "start")
                    {
                        keyForGamepadStart = reader.ReadElementContentAsString();
                    }
                    else if (reader.Name == "dpadLeft")
                    {
                        keyForGamepadDpadLeft = reader.ReadElementContentAsString();
                    }
                    else if (reader.Name == "dpadRight")
                    {
                        keyForGamepadDpadRight = reader.ReadElementContentAsString();
                    }
                    else if (reader.Name == "dpadDown")
                    {
                        keyForGamepadDpadDown = reader.ReadElementContentAsString();
                    }
                    else if (reader.Name == "dpadUp")
                    {
                        keyForGamepadDpadUp = reader.ReadElementContentAsString();
                    }
                }
            }

            return new CustomButtonMapping(
                keyForGamepadBack,
                keyForGamepadStart,
                keyForGamepadDpadLeft,
                keyForGamepadDpadRight,
                keyForGamepadDpadDown,
                keyForGamepadDpadUp);
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
            WriteHuluEntry(writer);
            WriteWebsiteEntry(writer, "Amazon Video", "https://www.amazon.com/video");
            WriteWebsiteEntry(writer, "HBO Max", "https://www.hbomax.com/");
            WriteProgramEntry(writer, "Steam Big Picture", "Steam", "C:\\Program Files (x86)\\Steam\\steam.exe", "-bigPicture", "steam://open/bigpicture", true);
            WriteWebsiteEntry(writer, "YouTube", "https://www.youtube.com");
            WriteWebsiteEntry(writer, "Crunchyroll", "https://www.crunchyroll.com");
            writer.WriteEndElement();
            writer.WriteStartElement("options");
            writer.WriteStartElement("browser");
            writer.WriteStartElement("processName");
            writer.WriteString("Edge");
            writer.WriteEndElement();
            writer.WriteStartElement("path");
            writer.WriteString(@"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe");
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

        /*
         * This is totally a hack, but in retrospect, I should have just provided a default config
         * file instead of writing it programmatically anyways.
         */
        private static void WriteHuluEntry(XmlWriter writer)
        {
            writer.WriteStartElement("website");
            writer.WriteStartElement("name");
            writer.WriteString("Hulu");
            writer.WriteEndElement();
            writer.WriteStartElement("url");
            writer.WriteString("https://www.hulu.com");
            writer.WriteEndElement();
            writer.WriteStartElement("buttonMapping");
            writer.WriteStartElement("start");
            writer.WriteString(" ");
            writer.WriteEndElement();
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

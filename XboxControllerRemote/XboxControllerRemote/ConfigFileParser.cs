using System.Collections.Generic;
using System.Xml;
using XboxControllerRemote.AppMenuItems;

namespace XboxControllerRemote
{
    public static class ConfigFileParser
    {
        public static string LoadBrowserPath()
        {
            XmlReader reader = XmlReader.Create("Config.xml");
            reader.ReadToNextSibling("configuration");
            reader.ReadToDescendant("options");
            reader.ReadToDescendant("browser");
            reader.ReadToDescendant("path");
            return reader.ReadElementContentAsString();
        }

        public static string LoadBrowserProcessName()
        {
            XmlReader reader = XmlReader.Create("Config.xml");
            reader.ReadToNextSibling("configuration");
            reader.ReadToDescendant("options");
            reader.ReadToDescendant("browser");
            reader.ReadToDescendant("processName");
            return reader.ReadElementContentAsString();
        }

        public static List<AppMenuItem> LoadMenuItems()
        {
            List<AppMenuItem> menuItems = new List<AppMenuItem>();
            XmlReader reader = XmlReader.Create("Config.xml");
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
    }
}

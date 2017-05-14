using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using XboxControllerRemote.AppMenuItems;

namespace XboxControllerRemote
{
    public class AppMenu : Menu
    {
        private List<AppMenuItem> menuItems;
        private int selectedIndex = 0;

        public AppMenu(MainForm form, int width, int height) : base(form, width, height)
        {
            menuItems = LoadMenuItems();
            menuItems.Add(new MouseEmulatorItem());
            menuItems.Add(new ShutdownItem());
            menuItems.Add(new QuitItem());
        }

        public override void Draw(Graphics graphics)
        {
            graphics.Clear(Color.LightGray);

            int menuItemHeight = height / menuItems.Count;

            for (int i = 0; i < menuItems.Count; i++)
            {
                int vOffset = i * height / menuItems.Count;
                Rectangle rect = new Rectangle(0, vOffset, width, menuItemHeight);
                Font font = new Font(MENU_FONT, 16);
                if (i == selectedIndex)
                {
                    graphics.FillRectangle(Brushes.Black, rect);
                    graphics.DrawString(menuItems[i].Name, font, Brushes.White, 0, vOffset);
                }
                else
                {
                    graphics.DrawRectangle(Pens.Black, rect);
                    graphics.DrawString(menuItems[i].Name, font, Brushes.Black, 0, vOffset);
                }
            }
        }

        public override void OnDownButton()
        {
            selectedIndex++;
            if (selectedIndex >= menuItems.Count)
            {
                selectedIndex = 0;
            }
        }

        public override void OnUpButton()
        {
            selectedIndex--;
            if (selectedIndex < 0)
            {
                selectedIndex = menuItems.Count - 1;
            }
        }

        public override void OnAButton()
        {
            AppMenuItem selectedItem = menuItems[selectedIndex];
            mainForm.StartApp(selectedItem);
        }

        public List<AppMenuItem> LoadMenuItems()
        {
            List<AppMenuItem> menuItems = new List<AppMenuItem>();
            XmlReader reader = null;
            try
            {
                reader = XmlReader.Create("Config.xml");
            }
            catch(System.IO.FileNotFoundException)
            {
                mainForm.ExitWithMessage("Config file not found.");
            }
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

        public WebsiteItem ParseWebsiteItem(XmlReader reader)
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

        public ProgramItem ParseProgramItem(XmlReader reader)
        {
            Dictionary<string, string> members = ParseProgramItemHelper(reader);
            return new ProgramItem(members["name"], members["processName"], members["processPath"], members["args"], members["appStartedArgs"]);
        }

        public ControllerProgramItem ParseControllerProgramItem(XmlReader reader)
        {
            Dictionary<string, string> members = ParseProgramItemHelper(reader);
            return new ControllerProgramItem(members["name"], members["processName"], members["processPath"], members["args"], members["appStartedArgs"]);
        }

        public Dictionary<string, string> ParseProgramItemHelper(XmlReader reader)
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

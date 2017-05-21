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
            try
            {
                menuItems = ConfigFileParser.LoadMenuItems();
            }
            catch (System.IO.FileNotFoundException)
            {
                mainForm.ExitWithMessage("Config file not found.");
            }
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
    }
}

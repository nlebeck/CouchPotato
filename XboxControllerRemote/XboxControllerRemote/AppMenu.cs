using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XboxControllerRemote
{
    public class AppMenu : Menu
    {
        private string[] menuItems = { "Netflix", "Hulu", "Steam", "Mouse Emulator", "Quit" };
        private Dictionary<string, string> urls = new Dictionary<string, string>()
        {
            { "Netflix", "https://www.netflix.com" },
            { "Hulu", "https://www.hulu.com" }
        };

        private int selectedIndex = 0;

        public AppMenu(MainForm form, int width, int height) : base(form, width, height) { }

        public override void Draw(Graphics graphics)
        {
            graphics.Clear(Color.LightGray);

            int menuItemHeight = height / menuItems.Length;

            for (int i = 0; i < menuItems.Length; i++)
            {
                int vOffset = i * height / menuItems.Length;
                Rectangle rect = new Rectangle(0, vOffset, width, menuItemHeight);
                Font font = new Font(MENU_FONT, 16);
                if (i == selectedIndex)
                {
                    graphics.FillRectangle(Brushes.Black, rect);
                    graphics.DrawString(menuItems[i], font, Brushes.White, 0, vOffset);
                }
                else
                {
                    graphics.DrawRectangle(Pens.Black, rect);
                    graphics.DrawString(menuItems[i], font, Brushes.Black, 0, vOffset);
                }
            }
        }

        public override void OnDownButton()
        {
            selectedIndex++;
            if (selectedIndex >= menuItems.Length)
            {
                selectedIndex = 0;
            }
        }

        public override void OnUpButton()
        {
            selectedIndex--;
            if (selectedIndex < 0)
            {
                selectedIndex = menuItems.Length - 1;
            }
        }

        public override void OnAButton()
        {
            string selectedItem = menuItems[selectedIndex];
            if (urls.ContainsKey(selectedItem))
            {
                mainForm.LaunchWebsite(urls[selectedItem]);
            }
            else if (selectedItem.Equals("Steam"))
            {
                mainForm.StartSteam();
            }
            else if (selectedItem.Equals("Mouse Emulator"))
            {
                mainForm.StartMouseEmulator();
            }
            else if (selectedItem.Equals("Quit"))
            {
                Application.Exit();
            }
        }
    }
}

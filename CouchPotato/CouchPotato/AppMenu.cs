using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using CouchPotato.AppMenuItems;

namespace CouchPotato
{
    public class AppMenu : Menu
    {
        private const int MENU_ITEMS_IN_COL = 4;

        private List<AppMenuItem> leftMenuItems;
        private List<AppMenuItem> rightMenuItems;
        private int selectedRow = 0;
        private bool rightColSelected = false;
        private int leftMenuOffset = 0;

        public AppMenu(MainForm form, int width, int height) : base(form, width, height)
        {
            try
            {
                leftMenuItems = ConfigFileParser.LoadMenuItems();
            }
            catch (System.IO.FileNotFoundException)
            {
                mainForm.Exit("Config file not found. Something must have prevented CouchPotato from creating a new default config file.");
            }

            rightMenuItems = new List<AppMenuItem>();
            rightMenuItems.Add(new MouseEmulatorItem());
            rightMenuItems.Add(new ShutdownItem());
            rightMenuItems.Add(new QuitItem());
        }

        public override void Draw(Graphics graphics)
        {
            graphics.Clear(BACKGROUND_COLOR);

            int menuItemWidth = width / 2;
            int menuItemHeight = height / 6;

            for (int i = 0; i < MENU_ITEMS_IN_COL; i++)
            {
                int vOffset = (i + 1) * height / 6;
                bool selected = (i == selectedRow && !rightColSelected);
                DrawMenuItem(graphics, leftMenuItems[i + leftMenuOffset], 0, vOffset, menuItemWidth, menuItemHeight, selected);
            }

            for (int i = 0; i < rightMenuItems.Count; i++)
            {
                int vOffset = (i + 1) * height / 6;
                bool selected = (i == selectedRow && rightColSelected);
                DrawMenuItem(graphics, rightMenuItems[i], width / 2, vOffset, menuItemWidth, menuItemHeight, selected);
            }

            if (leftMenuOffset > 0)
            {
                float p1x = width / 4;
                float p1y = height / 36;
                float p2x = 3 * width / 16;
                float p2y = 4 * height / 36;
                float p3x = 5 * width / 16;
                float p3y = 4 * height / 36;

                DrawTriangle(graphics, p1x, p1y, p2x, p2y, p3x, p3y);
            }

            if (leftMenuOffset + MENU_ITEMS_IN_COL < leftMenuItems.Count)
            {
                float p1x = width / 4;
                float p1y = height - height / 36;
                float p2x = 3 * width / 16;
                float p2y = height - 4 * height / 36;
                float p3x = 5 * width / 16;
                float p3y = height - 4 * height / 36;

                DrawTriangle(graphics, p1x, p1y, p2x, p2y, p3x, p3y);
            }
        }

        private void DrawTriangle(Graphics graphics, float p1x, float p1y, float p2x, float p2y, float p3x, float p3y)
        {
            graphics.DrawLine(Pens.Black, p1x, p1y, p2x, p2y);
            graphics.DrawLine(Pens.Black, p2x, p2y, p3x, p3y);
            graphics.DrawLine(Pens.Black, p3x, p3y, p1x, p1y);
        }

        private void DrawMenuItem(Graphics graphics, AppMenuItem menuItem, int x, int y, int width, int height, bool selected)
        {
            Rectangle rect = new Rectangle(x, y, width, height);
            Font font = new Font(MENU_FONT, fontSize);
            if (selected)
            {
                graphics.FillRectangle(Brushes.Black, rect);
                graphics.DrawString(menuItem.Name, font, Brushes.White, x, y);
            }
            else
            {
                graphics.DrawRectangle(Pens.Black, rect);
                graphics.DrawString(menuItem.Name, font, Brushes.Black, x, y);
            }
        }

        public override void OnDownButton()
        {
            if (rightColSelected)
            {
                if (selectedRow < rightMenuItems.Count - 1)
                {
                    selectedRow++;
                }
            }
            else
            {
                if (selectedRow < MENU_ITEMS_IN_COL - 1)
                {
                    selectedRow++;
                }
                else if (leftMenuOffset < leftMenuItems.Count - MENU_ITEMS_IN_COL)
                {
                    leftMenuOffset++;
                }
            }
        }

        public override void OnUpButton()
        {
            if (selectedRow > 0)
            {
                selectedRow--;
            }
            else if (!rightColSelected && leftMenuOffset > 0)
            {
                leftMenuOffset--;
            }
        }

        public override void OnRightButton()
        {
            if (!rightColSelected)
            {
                rightColSelected = true;
                if (selectedRow >= rightMenuItems.Count)
                {
                    selectedRow = rightMenuItems.Count - 1;
                }
            }
        }

        public override void OnLeftButton()
        {
            if (rightColSelected)
            {
                rightColSelected = false;
            }
        }

        public override void OnAButton()
        {
            AppMenuItem selectedItem = null;
            if (rightColSelected)
            {
                selectedItem = rightMenuItems[selectedRow];
            }
            else
            {
                selectedItem = leftMenuItems[selectedRow + leftMenuOffset];
            }
            mainForm.StartApp(selectedItem);
        }
    }
}

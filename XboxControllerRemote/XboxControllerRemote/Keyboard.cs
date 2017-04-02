using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace XboxControllerRemote
{
    public class Keyboard
    {
        public enum Direction { Left, Right, Up, Down };

        private static string FONT = "Arial";

        private string[][] keys;
        private int width;
        private int height;

        private int selectedCol;
        private int selectedRow;

        public Keyboard(int width, int height)
        {
            this.width = width;
            this.height = height;

            keys = new string[3][];
            keys[0] = new string[] { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P"};
            keys[1] = new string[] { "A", "S", "D", "F", "G", "H", "J", "K", "L", "{ENTER}" };
            keys[2] = new string[] { "Z", "X", "C", "V", "B", "N", "M" };

            selectedCol = 0;
            selectedRow = 0;
        }

        private int GetMaxRowLength()
        {
            int maxLength = 0;
            foreach(string[] row in keys)
            {
                if (row.Length > maxLength)
                {
                    maxLength = row.Length;
                }
            }
            return maxLength;
        }

        private int GetNumRows()
        {
            return keys.Length;
        }

        private int GetKeyWidth()
        {
            return width / GetMaxRowLength();
        }
        
        private int GetKeyHeight()
        {
            return height / GetNumRows();
        }

        public void DrawKeyboard(Graphics graphics)
        {
            graphics.Clear(Color.Gray);

            for (int row = 0; row < keys.Length; row++)
            {
                for (int col = 0; col < keys[row].Length; col++)
                {
                    string key = keys[row][col];

                    int x = GetKeyWidth() * col;
                    int y = GetKeyHeight() * row;

                    Rectangle rect = new Rectangle(x, y, GetKeyWidth(), GetKeyHeight());

                    int fontSize = GetKeyWidth() / 2;
                    int textX = x;
                    int textY = y + GetKeyHeight() / 4;
                    if (key.Length > 1)
                    {
                        fontSize = GetKeyWidth() / 7;
                        textY = y + GetKeyHeight() / 2;
                    }
                    Font font = new Font(FONT, fontSize);

                    if (col == selectedCol && row == selectedRow)
                    {
                        graphics.FillRectangle(Brushes.Black, rect);
                        graphics.DrawString(key, font, Brushes.White, textX, textY);
                    }
                    else
                    {
                        graphics.DrawRectangle(Pens.Black, rect);
                        graphics.DrawString(key, font, Brushes.Black, textX, textY);
                    }

                }
            }
        }

        public void MoveCursor(Direction direction)
        {
            int modifiedCol = selectedCol;
            int modifiedRow = selectedRow;

            switch (direction)
            {
                case Direction.Left:
                    modifiedCol -= 1;
                    break;
                case Direction.Right:
                    modifiedCol += 1;
                    break;
                case Direction.Up:
                    modifiedRow -= 1;
                    break;
                case Direction.Down:
                    modifiedRow += 1;
                    break;
            }

            Debug.WriteLine("Moving: {0}, {1}", modifiedCol, modifiedRow);

            if (modifiedRow < 0)
            {
                modifiedRow = 0;
            }
            if (modifiedRow >= GetNumRows())
            {
                modifiedRow = GetNumRows() - 1;
            }
            if (modifiedCol < 0)
            {
                modifiedCol = 0;
            }
            if (modifiedCol >= keys[modifiedRow].Length)
            {
                modifiedCol = keys[modifiedRow].Length - 1;
            }

            selectedCol = modifiedCol;
            selectedRow = modifiedRow;

            Debug.WriteLine("After: {0}, {1}", selectedCol, selectedRow);
        }

        public string GetSelectedKey()
        {
            return keys[selectedRow][selectedCol];
        }
    }
}

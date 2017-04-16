using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace XboxControllerRemote
{
    public class KeyboardMenu : Menu
    {
        public enum KeySet { Lowercase, Uppercase, Symbols };

        private Dictionary<KeySet, string[][]> keySets = new Dictionary<KeySet, string[][]>();

        private int selectedCol;
        private int selectedRow;
        KeySet currentKeySet;

        public KeyboardMenu(MainForm form, int width, int height) : base(form, width, height)
        {

            string[][] uppercaseKeys = new string[4][];
            uppercaseKeys[0] = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            uppercaseKeys[1] = new string[] { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P" };
            uppercaseKeys[2] = new string[] { "A", "S", "D", "F", "G", "H", "J", "K", "L", "{ENTER}" };
            uppercaseKeys[3] = new string[] { "Z", "X", "C", "V", "B", "N", "M", "@", "." };
            keySets.Add(KeySet.Uppercase, uppercaseKeys);

            string[][] lowercaseKeys = new string[4][];
            lowercaseKeys[0] = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            lowercaseKeys[1] = new string[] { "q", "w", "e", "r", "t", "y", "u", "i", "o", "p" };
            lowercaseKeys[2] = new string[] { "a", "s", "d", "f", "g", "h", "j", "k", "l", "{ENTER}" };
            lowercaseKeys[3] = new string[] { "z", "x", "c", "v", "b", "n", "m", "@", "." };
            keySets.Add(KeySet.Lowercase, lowercaseKeys);

            string[][] symbolKeys = new string[4][];
            symbolKeys[0] = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            symbolKeys[1] = new string[] { "!", "@", "#", "$", "%", "^", "&", "*", "(", ")" };
            symbolKeys[2] = new string[] { "-", "_", "=", "+", "[", "]", "{", "}", "'", "{ENTER}" };
            symbolKeys[3] = new string[] { "\\", "|", "<", ">", ",", ".", "?", "/", "\"" };
            keySets.Add(KeySet.Symbols, symbolKeys);

            selectedCol = 0;
            selectedRow = 0;
            currentKeySet = KeySet.Lowercase;
        }

        private int GetMaxRowLength()
        {
            int maxLength = 0;
            foreach (string[][] keys in keySets.Values)
            {
                foreach (string[] row in keys)
                {
                    if (row.Length > maxLength)
                    {
                        maxLength = row.Length;
                    }
                }
            }
            return maxLength;
        }

        private int GetMaxNumRows()
        {
            int maxNumRows = 0;
            foreach (string[][] keys in keySets.Values)
            {
                if (keys.Length > maxNumRows)
                {
                    maxNumRows = keys.Length;
                }
            }
            return maxNumRows;
        }

        private int GetKeyWidth()
        {
            return width / (GetMaxRowLength() + 1);
        }
        
        private int GetKeyHeight()
        {
            return height / (GetMaxNumRows() + 1);
        }

        public override void Draw(Graphics graphics)
        {
            graphics.Clear(Color.LightGray);

            for (int row = 0; row < GetCurrentKeys().Length; row++)
            {
                for (int col = 0; col < GetCurrentKeys()[row].Length; col++)
                {
                    string key = GetCurrentKeys()[row][col];

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
                    Font font = new Font(MENU_FONT, fontSize);

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

        public override void OnUpButton()
        {
            selectedRow -= 1;
            MoveSelectionInsideBounds();
        }

        public override void OnDownButton()
        {
            selectedRow += 1;
            MoveSelectionInsideBounds();
        }

        public override void OnLeftButton()
        {
            selectedCol -= 1;
            MoveSelectionInsideBounds();
        }

        public override void OnRightButton()
        {
            selectedCol += 1;
            MoveSelectionInsideBounds();
        }

        public override void OnRightShoulderButton()
        {
            if (currentKeySet == KeySet.Uppercase)
            {
                SwitchKeySet(KeySet.Lowercase);
            }
            else
            {
                SwitchKeySet(KeySet.Uppercase);
            }
        }

        public override void OnLeftShoulderButton()
        {
            if (currentKeySet == KeySet.Symbols)
            {
                SwitchKeySet(KeySet.Lowercase);
            }
            else
            {
                SwitchKeySet(KeySet.Symbols);
            }
        }

        public override void OnBackButton()
        {
            mainForm.SwitchToApp();
        }

        public override void OnAButton()
        {
            mainForm.SendKeyFromKeyboardMenu(GetSelectedKey());
        }

        public override void OnBButton()
        {
            mainForm.SendKeyFromKeyboardMenu("{BACKSPACE}");
        }

        private void MoveSelectionInsideBounds()
        {
            if (selectedRow < 0)
            {
                selectedRow = 0;
            }
            if (selectedRow >= GetCurrentKeys().Length)
            {
                selectedRow = GetCurrentKeys().Length - 1;
            }
            if (selectedCol < 0)
            {
                selectedCol = 0;
            }
            if (selectedCol >= GetCurrentKeys()[selectedRow].Length)
            {
                selectedCol = GetCurrentKeys()[selectedRow].Length - 1;
            }
        }

        private string[][] GetCurrentKeys()
        {
            return keySets[currentKeySet];
        }

        public string GetSelectedKey()
        {
            return GetCurrentKeys()[selectedRow][selectedCol];
        }

        public void SwitchKeySet(KeySet keySet)
        {
            currentKeySet = keySet;
            MoveSelectionInsideBounds();
        }
    }
}

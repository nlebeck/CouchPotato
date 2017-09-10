using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CouchPotato
{
    public abstract class Menu
    {
        public const string MENU_FONT = "Arial";
        public static Color BACKGROUND_COLOR = Color.LightGray;

        protected MainForm mainForm;
        protected int width;
        protected int height;
        protected int fontSize;

        public Menu(MainForm form, int width, int height)
        {
            this.mainForm = form;
            this.width = width;
            this.height = height;
            this.fontSize = GetFontSize(width);
        }

        public static int GetFontSize(int width)
        {
            return width / 60;
        }

        public abstract void Draw(Graphics graphics);

        public virtual void OnLeftButton() { }
        public virtual void OnRightButton() { }
        public virtual void OnUpButton() { }
        public virtual void OnDownButton() { } 
        public virtual void OnStartButton() { }
        public virtual void OnBackButton() { }
        public virtual void OnLeftShoulderButton() { }
        public virtual void OnRightShoulderButton() { }
        public virtual void OnAButton() { }
        public virtual void OnBButton() { }
        public virtual void OnXButton() { }
        public virtual void OnYButton() { }
    }
}

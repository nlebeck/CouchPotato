using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XboxControllerRemote
{
    public abstract class Menu
    {
        protected const string MENU_FONT = "Arial";

        protected MainForm mainForm;
        protected int width;
        protected int height;

        public Menu(MainForm form, int width, int height)
        {
            this.mainForm = form;
            this.width = width;
            this.height = height;
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

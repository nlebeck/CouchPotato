namespace CouchPotato
{
    using System;
    using System.Drawing;

    public class MessageMenu : Menu
    {
        public MessageMenu(MainForm form, int width, int height): base(form, width, height) {}

        public override void Draw(Graphics graphics)
        {
            Font font = new Font(MENU_FONT, fontSize);
            graphics.Clear(BACKGROUND_COLOR);
            graphics.DrawString(mainForm.CurrentMessage, font, Brushes.Black, new Point((int)(0.05 * width), (int)(0.2 * height)));
            graphics.DrawString("Press A to continue", font, Brushes.Black, new Point((int)(0.05 * width), mainForm.Height - (int)(0.4 * height)));
        }

        public override void OnAButton()
        {
            mainForm.ChangeMenu(typeof(AppMenu));
        }
    }
}

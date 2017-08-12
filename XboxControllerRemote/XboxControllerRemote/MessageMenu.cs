namespace XboxControllerRemote
{
    using System;
    using System.Drawing;

    public class MessageMenu : Menu
    {
        public MessageMenu(MainForm form, int width, int height): base(form, width, height) {}

        public override void Draw(Graphics graphics)
        {
            Font font = new Font(MENU_FONT, MENU_FONT_SIZE);
            graphics.Clear(BACKGROUND_COLOR);
            graphics.DrawString(mainForm.CurrentMessage, font, Brushes.Black, new Point(100, 100));
            graphics.DrawString("Press A to continue", font, Brushes.Black, new Point(100, mainForm.Height - 200));
        }

        public override void OnAButton()
        {
            mainForm.ChangeMenu(typeof(AppMenu));
        }
    }
}

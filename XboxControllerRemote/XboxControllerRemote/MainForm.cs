using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using XInputWrapper;

namespace XboxControllerRemote
{
    public partial class MainForm : Form
    {
        delegate void detectInputDelegate();

        public MainForm()
        {
            InitializeComponent();
            System.Timers.Timer timer = new System.Timers.Timer(10);
            timer.Elapsed += (sender, e) => Invoke(new detectInputDelegate(DetectInput));
            timer.Start();
        }

        public void DetectInput()
        {
            this.Cursor = new Cursor(Cursor.Current.Handle);

            int offsetX = 0;
            int offsetY = 0;
            XInputState state = XInputState.XInputGetStateWrapper(0);
            if (Math.Abs((int)state.Gamepad.sThumbLX) > XInputState.LEFT_THUMB_DEADZONE)
            {
                offsetX = (int)(state.Gamepad.sThumbLX / 32767.0 * 25.0);
            }
            if (Math.Abs((int)state.Gamepad.sThumbLY) > XInputState.LEFT_THUMB_DEADZONE)
            {
                offsetY = (int)(state.Gamepad.sThumbLY / 32767.0 * 25.0);
            }
            Cursor.Position = new Point(Cursor.Position.X + offsetX, Cursor.Position.Y - offsetY);
        }
    }
}

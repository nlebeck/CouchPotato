using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using XInputWrapper;

namespace XboxControllerRemote
{
    public partial class MainForm : Form
    {
        delegate void detectInputDelegate();

        private XInputState prevState;
        private System.Timers.Timer timer;

        public MainForm()
        {
            InitializeComponent();

            this.Cursor = new Cursor(Cursor.Current.Handle);

            timer = new System.Timers.Timer(10);
            timer.Elapsed += (sender, e) => Invoke(new detectInputDelegate(DetectInput));
            timer.Start();
        }

        private bool buttonPressed(XInputState state, XInputState prevState, ushort buttonMask)
        {
            return((state.Gamepad.wButtons & buttonMask) > 0 && (prevState.Gamepad.wButtons & buttonMask) == 0) ;
        }

        public void DetectInput()
        {
            XInputState state = XInputState.XInputGetStateWrapper(0);

            int offsetX = 0;
            int offsetY = 0;
            if (Math.Abs((int)state.Gamepad.sThumbLX) > XInputConstants.GAMEPAD_LEFT_THUMB_DEADZONE)
            {
                offsetX = (int)(state.Gamepad.sThumbLX / 32767.0 * 25.0);
            }
            if (Math.Abs((int)state.Gamepad.sThumbLY) > XInputConstants.GAMEPAD_LEFT_THUMB_DEADZONE)
            {
                offsetY = (int)(state.Gamepad.sThumbLY / 32767.0 * 25.0);
            }
            Cursor.Position = new Point(Cursor.Position.X + offsetX, Cursor.Position.Y - offsetY);

            if (buttonPressed(state, prevState, XInputConstants.GAMEPAD_A))
            {
                MouseEventWrapper.MouseEvent(MouseEventWrapper.MOUSEEVENTTF_LEFTDOWN, 0, 0, 0, 0);
                MouseEventWrapper.MouseEvent(MouseEventWrapper.MOUSEEVENTTF_LEFTUP, 0, 0, 0, 0);
            }

            prevState = state;
        }
    }
}

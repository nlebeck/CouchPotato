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
        private const string BROWSER_FILE_NAME = "IExplore.exe";
        private const string NETFLIX_URL = "https://www.netflix.com";
        private const int BUTTON_PRESS_SLEEP_MS = 100;
        private const int POLLING_INTERVAL_MS = 10;

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hwnd);

        delegate void detectInputDelegate();

        private XInputState prevState;
        private System.Timers.Timer timer;
        private Process browserProcess = null;
        private bool inKeyboardMode = false;

        public MainForm()
        {
            InitializeComponent();

            this.Cursor = new Cursor(Cursor.Current.Handle);

            timer = new System.Timers.Timer(POLLING_INTERVAL_MS);
            timer.Elapsed += (sender, e) => Invoke(new detectInputDelegate(DetectInput));
            timer.Start();
        }

        private bool ButtonPressed(XInputState state, XInputState prevState, ushort buttonMask)
        {
            return((state.Gamepad.wButtons & buttonMask) > 0 && (prevState.Gamepad.wButtons & buttonMask) == 0) ;
        }

        public void DetectInput()
        {
            XInputState state = XInputState.XInputGetStateWrapper(0);

            if (inKeyboardMode)
            {
                if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_BACK))
                {
                    inKeyboardMode = false;
                    SetForegroundWindow(browserProcess.MainWindowHandle);
                }
                else if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_A))
                {
                    SetForegroundWindow(browserProcess.MainWindowHandle);
                    Thread.Sleep(BUTTON_PRESS_SLEEP_MS);
                    SendKeys.Send("A");
                    Thread.Sleep(BUTTON_PRESS_SLEEP_MS);
                    SetForegroundWindow(Process.GetCurrentProcess().MainWindowHandle);
                }
            }
            else
            {
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

                if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_A))
                {
                    MouseEventWrapper.MouseEvent(MouseEventWrapper.MOUSEEVENTTF_LEFTDOWN, 0, 0, 0, 0);
                    MouseEventWrapper.MouseEvent(MouseEventWrapper.MOUSEEVENTTF_LEFTUP, 0, 0, 0, 0);
                }
                else if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_START))
                {
                    if (browserProcess == null)
                    {
                        browserProcess = Process.Start(BROWSER_FILE_NAME, NETFLIX_URL);
                    }
                }
                else if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_BACK))
                {
                    inKeyboardMode = true;
                    SetForegroundWindow(Process.GetCurrentProcess().MainWindowHandle);
                }
            }

            prevState = state;
        }
    }
}

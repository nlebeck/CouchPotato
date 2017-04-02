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

        private const int HRES = 1920;
        private const double KEYBOARD_WIDTH_SCALE = 0.5;
        private const double KEYBOARD_ASPECT_RATIO = 16.0 / 9.0;

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hwnd);

        delegate void detectInputDelegate();

        private XInputState prevState;
        private System.Timers.Timer timer;
        private Process browserProcess = null;
        private Keyboard keyboard;

        private bool inKeyboardMode = false;

        public MainForm()
        {
            InitializeComponent();

            this.Cursor = new Cursor(Cursor.Current.Handle);

            timer = new System.Timers.Timer(POLLING_INTERVAL_MS);
            timer.Elapsed += (sender, e) => Invoke(new detectInputDelegate(DetectInput));
            timer.Start();

            Width = (int)(HRES * KEYBOARD_WIDTH_SCALE);
            Height = (int)(Width / KEYBOARD_ASPECT_RATIO);

            keyboard = new Keyboard(Width, Height);
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
                Graphics formGraphics = CreateGraphics();
                keyboard.DrawKeyboard(formGraphics);
                formGraphics.Dispose();

                if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_BACK))
                {
                    inKeyboardMode = false;
                    SetForegroundWindow(browserProcess.MainWindowHandle);
                }
                else if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_DPAD_LEFT))
                {
                    keyboard.MoveCursor(Keyboard.Direction.Left);
                }
                else if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_DPAD_RIGHT))
                {
                    keyboard.MoveCursor(Keyboard.Direction.Right);
                }
                else if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_DPAD_UP))
                {
                    keyboard.MoveCursor(Keyboard.Direction.Up);
                }
                else if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_DPAD_DOWN))
                {
                    keyboard.MoveCursor(Keyboard.Direction.Down);
                }
                else if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_A))
                {
                    SetForegroundWindow(browserProcess.MainWindowHandle);
                    Thread.Sleep(BUTTON_PRESS_SLEEP_MS);
                    SendKeys.Send(keyboard.GetSelectedKey());
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

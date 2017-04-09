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
        private enum State { Menu, App, Keyboard }

        private const string BROWSER_FILE_NAME = "IExplore.exe";
        private const string NETFLIX_URL = "https://www.netflix.com";

        private const int BUTTON_PRESS_SLEEP_MS = 50;
        private const int POLLING_INTERVAL_MS = 10;
        private const int KEYBOARD_POLLING_INTERVAL_MS = 50;

        private const int HRES = 1920;
        private const double KEYBOARD_WIDTH_SCALE = 0.5;
        private const double KEYBOARD_ASPECT_RATIO = 16.0 / 9.0;

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hwnd);

        delegate void detectInputDelegate();

        private XInputState prevState;
        private System.Timers.Timer timer;
        private Process browserProcess = null;
        private BufferedGraphics buffer;
        private Keyboard keyboard;
        private Menu currentMenu;
        private State currentState;

        public MainForm()
        {
            InitializeComponent();

            this.Cursor = new Cursor(Cursor.Current.Handle);

            Width = (int)(HRES * KEYBOARD_WIDTH_SCALE);
            Height = (int)(Width / KEYBOARD_ASPECT_RATIO);

            keyboard = new Keyboard(Width, Height);
            currentMenu = new AppMenu(this, Width, Height);
            currentState = State.Menu;

            buffer = BufferedGraphicsManager.Current.Allocate(CreateGraphics(), new Rectangle(0, 0, Width, Height));

            timer = new System.Timers.Timer(POLLING_INTERVAL_MS);
            timer.Elapsed += (sender, e) => Invoke(new detectInputDelegate(DetectInput));
            timer.Start();
        }

        private bool ButtonPressed(XInputState state, XInputState prevState, ushort buttonMask)
        {
            return((state.Gamepad.wButtons & buttonMask) > 0 && (prevState.Gamepad.wButtons & buttonMask) == 0) ;
        }

        private bool ButtonReleased(XInputState state, XInputState prevState, ushort buttonMask)
        {
            return ((state.Gamepad.wButtons & buttonMask) == 0 && (prevState.Gamepad.wButtons & buttonMask) > 0);
        }

        public void LaunchWebsite(string url)
        {
            browserProcess = Process.Start(BROWSER_FILE_NAME, url);
            SetForegroundWindow(browserProcess.MainWindowHandle);
            currentState = State.App;
        }

        public void DetectInput()
        {
            XInputState state = XInputState.XInputGetStateWrapper(0);

            if (currentState == State.Menu)
            {
                Graphics formGraphics = CreateGraphics();
                currentMenu.Draw(buffer.Graphics);
                buffer.Render(formGraphics);
                formGraphics.Dispose();

                if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_A))
                {
                    currentMenu.OnAButton();
                }
                if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_B))
                {
                    currentMenu.OnBButton();
                }
                if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_START))
                {
                    currentMenu.OnStartButton();
                }
                if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_BACK))
                {
                    currentMenu.OnBackButton();
                }
                if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_DPAD_UP))
                {
                    currentMenu.OnUpButton();
                }
                if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_DPAD_DOWN))
                {
                    currentMenu.OnDownButton();
                }
                if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_DPAD_LEFT))
                {
                    currentMenu.OnLeftButton();
                }
                if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_DPAD_RIGHT))
                {
                    currentMenu.OnRightButton();
                }
                if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_LEFT_SHOULDER))
                {
                    currentMenu.OnLeftShoulderButton();
                }
                if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_RIGHT_SHOULDER))
                {
                    currentMenu.OnRightShoulderButton();
                }
            }
            else if (currentState == State.Keyboard)
            {
                Graphics formGraphics = CreateGraphics();
                keyboard.DrawKeyboard(buffer.Graphics);
                buffer.Render(formGraphics);
                formGraphics.Dispose();

                string keyToSend = null;

                if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_BACK))
                {
                    currentState = State.App;
                    timer.Interval = POLLING_INTERVAL_MS;
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
                    keyToSend = keyboard.GetSelectedKey();
                }
                else if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_B))
                {
                    keyToSend = "{BACKSPACE}";
                }
                else if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_RIGHT_SHOULDER))
                {
                    if (keyboard.CurrentKeySet() == Keyboard.KeySet.Uppercase)
                    {
                        keyboard.SwitchKeySet(Keyboard.KeySet.Lowercase);
                    }
                    else
                    {
                        keyboard.SwitchKeySet(Keyboard.KeySet.Uppercase);
                    }
                }
                else if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_LEFT_SHOULDER))
                {
                    if (keyboard.CurrentKeySet() == Keyboard.KeySet.Symbols)
                    {
                        keyboard.SwitchKeySet(Keyboard.KeySet.Lowercase);
                    }
                    else
                    {
                        keyboard.SwitchKeySet(Keyboard.KeySet.Symbols);
                    }
                }

                if (keyToSend != null)
                {
                    SetForegroundWindow(browserProcess.MainWindowHandle);
                    Thread.Sleep(BUTTON_PRESS_SLEEP_MS);
                    SendKeys.Send(keyToSend);
                    Thread.Sleep(BUTTON_PRESS_SLEEP_MS);
                    SetForegroundWindow(Process.GetCurrentProcess().MainWindowHandle);
                }
            }
            else if (currentState == State.App)
            {
                int offsetX = 0;
                int offsetY = 0;
                GetMouseOffsets(state.Gamepad.sThumbLX, state.Gamepad.sThumbLY, out offsetX, out offsetY);
                Cursor.Position = new Point(Cursor.Position.X + offsetX, Cursor.Position.Y - offsetY);

                if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_A))
                {
                    MouseEventWrapper.MouseEvent(MouseEventWrapper.FLAG_MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                }
                else if (ButtonReleased(state, prevState, XInputConstants.GAMEPAD_A))
                {
                    MouseEventWrapper.MouseEvent(MouseEventWrapper.FLAG_MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                }
                else if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_B))
                {
                    MouseEventWrapper.MouseEvent(MouseEventWrapper.FLAG_MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
                }
                else if (ButtonReleased(state, prevState, XInputConstants.GAMEPAD_B))
                {
                    MouseEventWrapper.MouseEvent(MouseEventWrapper.FLAG_MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
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
                    currentState = State.Keyboard;
                    timer.Interval = KEYBOARD_POLLING_INTERVAL_MS;
                    SetForegroundWindow(Process.GetCurrentProcess().MainWindowHandle);
                }
                else if (state.Gamepad.bLeftTrigger > XInputConstants.GAMEPAD_TRIGGER_THRESHOLD)
                {
                    MouseEventWrapper.MouseEvent(MouseEventWrapper.FLAG_MOUSEEVENTF_WHEEL, 0, 0, MouseEventWrapper.VALUE_WHEEL_DELTA / 8, 0);
                }
                else if (state.Gamepad.bRightTrigger > XInputConstants.GAMEPAD_TRIGGER_THRESHOLD)
                {
                    MouseEventWrapper.MouseEvent(MouseEventWrapper.FLAG_MOUSEEVENTF_WHEEL, 0, 0, (uint)(-MouseEventWrapper.VALUE_WHEEL_DELTA / 8), 0);
                }
            }

            if (browserProcess != null && browserProcess.HasExited)
            {
                Application.Exit();
            }

            prevState = state;
        }

        public void GetMouseOffsets(short rawThumbX, short rawThumbY, out int offsetX, out int offsetY)
        {
            double scaledThumbX = rawThumbX / 32767.0;
            double scaledThumbY = rawThumbY / 32767.0;

            if (Math.Abs((int)rawThumbX) <= XInputConstants.GAMEPAD_LEFT_THUMB_DEADZONE)
            {
                scaledThumbX = 0.0;
            }
            if (Math.Abs((int)rawThumbY) <= XInputConstants.GAMEPAD_LEFT_THUMB_DEADZONE)
            {
                scaledThumbY = 0.0;
            }

            double scaledMagnitude = Math.Sqrt(Math.Pow(scaledThumbX, 2) + Math.Pow(scaledThumbY, 2));
            double multiplier = scaledMagnitude * 25.0;

            offsetX = (int)(scaledThumbX * multiplier);
            offsetY = (int)(scaledThumbY * multiplier);
        }
    }
}

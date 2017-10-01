using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Speech.Recognition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using XInputWrapper;
using CouchPotato.AppMenuItems;
using System.Media;
using System.Text;

namespace CouchPotato
{
    public partial class MainForm : Form
    {
        private const int BUTTON_PRESS_SLEEP_MS = 50;

        private const double ASPECT_RATIO = 16.0 / 9.0;

        private const double THUMB_MAX = 32767.0;
        private const double MOUSE_WHEEL_MULTIPLIER = 0.125;
        private const double MOUSE_MULTIPLIER = 12.5;

        public string CurrentMessage { get; set; }

        private static readonly Dictionary<State, int> POLLING_INTERVALS_MS = new Dictionary<State, int>() {
            { State.Menu, 10 }, { State.App, 10 }, { State.Disabled, 1000 }, { State.MouseEmulator, 10 }
        };

        private static readonly string[] DETECTED_WORDS = { "Alpha", "Bravo", "Charlie", "Delta", "Echo",
            "Foxtrot", "Golf", "Hotel", "India", "Juliett", "Kilo", "Lima", "Mike", "November",
            "Oscar", "Papa", "Quebec", "Romeo", "Sierra", "Tango", "Uniform", "Victor", "Whiskey",
            "Xray", "Yankee", "Zulu", "One", "Two", "Three", "Four", "Five", "Six", "Seven",
            "Eight", "Nine", "Zero", "Backspace", "Space" };

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hwnd);

        delegate void detectInputDelegate();

        private XInputState prevState;
        private System.Timers.Timer timer;
        private Process appProcess = null;
        private BufferedGraphics buffer;
        private Menu currentMenu;
        private State currentState;

        private string browserProcessPath;
        private string browserProcessName;
        private bool exiting = false;

        private SpeechRecognitionEngine speechEngine = null;
        private bool speechModeEnabled = true;
        private bool speechModeOn = false;
        private bool recognizingSpeech = false;

        public MainForm()
        {
            InitializeComponent();

            Width = ConfigFileParser.LoadWidth();
            Height = (int)(Width / ASPECT_RATIO);

            currentMenu = new AppMenu(this, ClientRectangle.Width, ClientRectangle.Height);
            currentState = State.Menu;

            buffer = BufferedGraphicsManager.Current.Allocate(CreateGraphics(), new Rectangle(0, 0, Width, Height));

            speechEngine = new SpeechRecognitionEngine();
            try
            {
                speechEngine.SetInputToDefaultAudioDevice();
            }
            catch (InvalidOperationException)
            {
                speechModeEnabled = false;
                Debug.WriteLine("Speech input disabled: no microphone detected");
            }
            if (speechModeEnabled)
            {
                Choices letters = new Choices();
                letters.Add(DETECTED_WORDS);
                GrammarBuilder gb = new GrammarBuilder();
                gb.Append(letters);
                Grammar grammar = new Grammar(gb);

                speechEngine.LoadGrammar(grammar);
                speechEngine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(OnSpeechRecognized);
                speechEngine.RecognizeCompleted += new EventHandler<RecognizeCompletedEventArgs>(OnRecognizeCompleted);
            }

            FormClosing += FormClosingHandler;

            timer = new System.Timers.Timer(POLLING_INTERVALS_MS[currentState]);
            timer.Elapsed += (sender, e) => Invoke(new detectInputDelegate(DetectInput));
            timer.Start();

            browserProcessName = ConfigFileParser.LoadBrowserProcessName();
            browserProcessPath = ConfigFileParser.LoadBrowserPath();
        }

        private void OnSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string word = e.Result.Text;
            string key = null;
            if (word.Equals("Backspace"))
            {
                key = "{BACKSPACE}";
            }
            else if (word.Equals("Space"))
            {
                key = " ";
            }
            else if (word.Equals("Zero"))
            {
                key = "0";
            }
            else if (word.Equals("One"))
            {
                key = "1";
            }
            else if (word.Equals("Two"))
            {
                key = "2";
            }
            else if (word.Equals("Three"))
            {
                key = "3";
            }
            else if (word.Equals("Four"))
            {
                key = "4";
            }
            else if (word.Equals("Five"))
            {
                key = "5";
            }
            else if (word.Equals("Six"))
            {
                key = "6";
            }
            else if (word.Equals("Seven"))
            {
                key = "7";
            }
            else if (word.Equals("Eight"))
            {
                key = "8";
            }
            else if (word.Equals("Nine"))
            {
                key = "9";
            }
            else
            {
                key = e.Result.Text.Substring(0, 1);
            }
            SendKeys.Send(key);
        }

        private void OnRecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            recognizingSpeech = false;
        }

        private void BeginSpeechMode()
        {
            if (speechModeEnabled)
            {
                speechModeOn = true;
                SystemSounds.Beep.Play();
            }
        }

        private void EndSpeechMode()
        {
            if (speechModeEnabled)
            {
                speechModeOn = false;
                recognizingSpeech = false;
                speechEngine.RecognizeAsyncCancel();
                SystemSounds.Beep.Play();
            }
        }

        private bool ButtonPressed(XInputState state, XInputState prevState, ushort buttonMask)
        {
            return((state.Gamepad.wButtons & buttonMask) > 0 && (prevState.Gamepad.wButtons & buttonMask) == 0) ;
        }

        private bool ButtonReleased(XInputState state, XInputState prevState, ushort buttonMask)
        {
            return ((state.Gamepad.wButtons & buttonMask) == 0 && (prevState.Gamepad.wButtons & buttonMask) > 0);
        }

        public void StartApp(AppMenuItem menuItem)
        {
            if (menuItem is WebsiteItem)
            {
                Process[] processes = Process.GetProcessesByName(browserProcessName);
                if (processes.Length > 0)
                {
                    DisplayMessage("Error: a browser window is open. Please close all open browser windows before launching a website through this program.");
                    return;
                }

                WebsiteItem websiteItem = (WebsiteItem)menuItem;
                try
                {
                    appProcess = Process.Start(browserProcessPath, websiteItem.Url);
                }
                catch
                {
                    DisplayMessage("Error launching website. Check to make sure the browser path is set correctly in the config file.");
                    return;
                }
                SwitchToState(State.App);
                ChangeMenu(typeof(KeyboardMenu));
            }
            else if (menuItem is ProgramItem)
            {
                ProgramItem programItem = (ProgramItem)menuItem;

                Process[] processes = Process.GetProcessesByName(programItem.ProcessName);
                if (processes.Length > 0)
                {
                    if (programItem.AppStartedArgs == null)
                    {
                        DisplayMessage("Error: This app is already running, and the config file entry for the app does not have an appStartedArgs element.");
                        return;
                    }
                    else
                    {
                        appProcess = processes[0];
                        try
                        {
                            Process.Start(programItem.ProcessPath, programItem.AppStartedArgs);
                        }
                        catch
                        {
                            DisplayMessage("Error starting app. Check to make sure the config file entry for this app has the correct path.");
                            return;
                        }
                    }
                }
                else
                {
                    try
                    {
                        appProcess = Process.Start(programItem.ProcessPath, programItem.Args);
                    }
                    catch
                    {
                        DisplayMessage("Error starting app. Check to make sure the config file entry for this app has the correct path.");
                        return;
                    }
                }


                if (programItem is ControllerProgramItem)
                {
                    SwitchToState(State.Disabled);
                }
                else
                {
                    SwitchToState(State.App);
                    ChangeMenu(typeof(KeyboardMenu));
                }
            }
            else if (menuItem is MouseEmulatorItem)
            {
                SwitchToState(State.MouseEmulator);
                ChangeMenu(typeof(AppMenu));
            }
            else if (menuItem is ShutdownItem)
            {
                Process.Start("shutdown", "/s /t 0");
            }
            else if (menuItem is QuitItem)
            {
                Exit(null);
            }
        }

        public void ChangeMenu(Type menu)
        {
            currentMenu = (Menu)Activator.CreateInstance(menu, this, ClientRectangle.Width, ClientRectangle.Height);
        }

        public void SwitchToState(State state)
        {
            currentState = state;
            timer.Interval = POLLING_INTERVALS_MS[state];

            if (state == State.App)
            {
                SetForegroundWindow(appProcess.MainWindowHandle);
            }
            else if (state == State.Menu)
            {
                SetForegroundWindow(Process.GetCurrentProcess().MainWindowHandle);
            }
        }

        public void SendKeyFromKeyboardMenu(string key)
        {
            IntPtr appWindowHandle = IntPtr.Zero;
            try
            {
                appWindowHandle = appProcess.MainWindowHandle;
            }
            catch (InvalidOperationException)
            {
                return;
            }
            SetForegroundWindow(appWindowHandle);
            Thread.Sleep(BUTTON_PRESS_SLEEP_MS);
            SendKeys.Send(key);
            Thread.Sleep(BUTTON_PRESS_SLEEP_MS);
            SetForegroundWindow(Process.GetCurrentProcess().MainWindowHandle);
        }

        public void FormClosingHandler(object sender, FormClosingEventArgs e)
        {
            // Exit through the Exit() method so that it can perform its cleanup
            if (!exiting)
            {
                e.Cancel = true;
                Exit(null);
            }
        }

        public void Exit(string message)
        {
            exiting = true;
            if (timer != null)
            {
                timer.Stop();
            }
            if (message != null)
            {
                MessageBox.Show("Exiting program: " + message);
            }
            Application.Exit();
        }

        public void DisplayMessage(string message)
        {
            string[] split = message.Split(' ');
            StringBuilder stringBuilder = new StringBuilder();
            int charsOnLine = 0;
            foreach (string word in split)
            {
                if (stringBuilder.Length != 0)
                {
                    if (charsOnLine + word.Length >= 40)
                    {
                        stringBuilder.Append("\n");
                        charsOnLine = 0;
                    }
                    else
                    {
                        stringBuilder.Append(" ");
                    }
                }
                stringBuilder.Append(word);
                charsOnLine += word.Length;
            }

            CurrentMessage = stringBuilder.ToString();
            ChangeMenu(typeof(MessageMenu));
        }

        // We don't switch to a MessageMenu because we want the user to be able to resume where
        // she left off once the controller is plugged back in
        public void HandleUnpluggedController()
        {
            Font font = new Font(CouchPotato.Menu.MENU_FONT, CouchPotato.Menu.GetFontSize(Width));
            buffer.Graphics.Clear(CouchPotato.Menu.BACKGROUND_COLOR);
            buffer.Graphics.DrawString("No XInput-compatible controller plugged in.", font, Brushes.Black, new Point((int)(0.05 * Width), (int)(0.2 * Height)));
            buffer.Graphics.DrawString("Plug in controller to continue", font, Brushes.Black, new Point((int)(0.05 * Width), Height - (int)(0.4 * Height)));
            Graphics graphics = CreateGraphics();
            buffer.Render(graphics);
            graphics.Dispose();
        }

        public void DetectInput()
        {
            if (exiting)
            {
                return;
            }

            XInputState state = new XInputState();
            uint ret = XInputState.XInputGetStateWrapper(0, ref state);
            if (ret != XInputConstants.RET_ERROR_SUCCESS)
            {
                HandleUnpluggedController();
                return;
            }

            Graphics formGraphics = CreateGraphics();
            currentMenu.Draw(buffer.Graphics);
            buffer.Render(formGraphics);
            formGraphics.Dispose();

            if (currentState == State.Menu)
            {
                int offsetX = 0;
                int offsetY = 0;
                GetMovementOffsets(state.Gamepad.sThumbRX, state.Gamepad.sThumbRY, out offsetX, out offsetY);

                this.Location = new Point(this.Location.X + offsetX, this.Location.Y - offsetY);

                if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_A))
                {
                    currentMenu.OnAButton();
                }
                if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_B))
                {
                    currentMenu.OnBButton();
                }
                if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_X))
                {
                    currentMenu.OnXButton();
                }
                if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_Y))
                {
                    currentMenu.OnYButton();
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
            else if (currentState == State.App || currentState == State.MouseEmulator)
            {
                int offsetX = 0;
                int offsetY = 0;
                GetMovementOffsets(state.Gamepad.sThumbLX, state.Gamepad.sThumbLY, out offsetX, out offsetY);
                MouseEventWrapper.MouseEvent(MouseEventWrapper.FLAG_MOUSEEVENTF_MOVE, (uint)offsetX, (uint)-offsetY, 0, 0);

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
                else if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_BACK))
                {
                    SendKeys.Send("{ESC}");
                }
                else if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_START))
                {
                    SendKeys.Send("{ENTER}");
                }
                else if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_DPAD_LEFT))
                {
                    SendKeys.Send("{LEFT}");
                }
                else if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_DPAD_RIGHT))
                {
                    SendKeys.Send("{RIGHT}");
                }
                else if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_DPAD_DOWN))
                {
                    SendKeys.Send("{DOWN}");
                }
                else if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_DPAD_UP))
                {
                    SendKeys.Send("{UP}");
                }
                else if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_Y))
                {
                    SwitchToState(State.Menu);
                }
                else if (ButtonPressed(state, prevState, XInputConstants.GAMEPAD_X))
                {
                    BeginSpeechMode();
                }
                else if (ButtonReleased(state, prevState, XInputConstants.GAMEPAD_X))
                {
                    EndSpeechMode();
                }
                else if (state.Gamepad.bLeftTrigger > XInputConstants.GAMEPAD_TRIGGER_THRESHOLD)
                {
                    MouseEventWrapper.MouseEvent(MouseEventWrapper.FLAG_MOUSEEVENTF_WHEEL, 0, 0, (uint)(MouseEventWrapper.VALUE_WHEEL_DELTA * MOUSE_WHEEL_MULTIPLIER), 0);
                }
                else if (state.Gamepad.bRightTrigger > XInputConstants.GAMEPAD_TRIGGER_THRESHOLD)
                {
                    MouseEventWrapper.MouseEvent(MouseEventWrapper.FLAG_MOUSEEVENTF_WHEEL, 0, 0, (uint)(-MouseEventWrapper.VALUE_WHEEL_DELTA * MOUSE_WHEEL_MULTIPLIER), 0);
                }
            }

            if (speechModeOn && !recognizingSpeech)
            {
                speechEngine.RecognizeAsync();
                recognizingSpeech = true;
            }

            if (appProcess != null && appProcess.HasExited)
            {
                appProcess = null;
                SwitchToState(State.Menu);
                ChangeMenu(typeof(AppMenu));
            }

            prevState = state;
        }

        public void GetMovementOffsets(short rawThumbX, short rawThumbY, out int offsetX, out int offsetY)
        {
            double scaledThumbX = rawThumbX / THUMB_MAX;
            double scaledThumbY = rawThumbY / THUMB_MAX;

            if (Math.Abs((int)rawThumbX) <= XInputConstants.GAMEPAD_LEFT_THUMB_DEADZONE)
            {
                scaledThumbX = 0.0;
            }
            if (Math.Abs((int)rawThumbY) <= XInputConstants.GAMEPAD_LEFT_THUMB_DEADZONE)
            {
                scaledThumbY = 0.0;
            }

            double scaledMagnitude = Math.Sqrt(Math.Pow(scaledThumbX, 2) + Math.Pow(scaledThumbY, 2));
            double multiplier = scaledMagnitude * MOUSE_MULTIPLIER;

            offsetX = (int)(scaledThumbX * multiplier);
            offsetY = (int)(scaledThumbY * multiplier);
        }
    }
}

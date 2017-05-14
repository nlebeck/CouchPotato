namespace XboxControllerRemote.AppMenuItems
{
    public class ProgramItem : AppMenuItem
    {
        public string ProcessName { get; set; }
        public string ProcessPath { get; set; }
        public string Args { get; set; }
        public string AppStartedArgs { get; set; }

        public ProgramItem(string name, string processName, string processPath, string args, string appStartedArgs)
            : base(name)
        {
            ProcessName = processName;
            ProcessPath = processPath;
            Args = args;
            AppStartedArgs = appStartedArgs;
        }
    }
}

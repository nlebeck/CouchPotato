namespace CouchPotato.AppMenuItems
{
    /// <summary>
    /// A program that has native controller support, so mouse/keyboard emulation is disabled.
    /// </summary>
    public class ControllerProgramItem : ProgramItem
    {
        public ControllerProgramItem(string name, string processName, string processPath, string args, string appStartedArgs)
            : base(name, processName, processPath, args, appStartedArgs)
        {
        }
    }
}

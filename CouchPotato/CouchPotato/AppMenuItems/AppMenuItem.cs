using System.Diagnostics;

namespace XboxControllerRemote.AppMenuItems
{
    public abstract class AppMenuItem
    {
        public string Name { get; set; }

        public AppMenuItem(string name)
        {
            Name = name;
        }
    }
}

using System.Diagnostics;

namespace CouchPotato.AppMenuItems
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

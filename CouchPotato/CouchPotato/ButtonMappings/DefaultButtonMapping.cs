using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CouchPotato.ButtonMappings
{
    public class DefaultButtonMapping : ButtonMapping
    {
        public virtual string GetKeyForGamepadBack()
        {
            return "{ESC}";
        }

        public virtual string GetKeyForGamepadStart()
        {
            return "{ENTER}";
        }

        public virtual string GetKeyForGamepadDpadLeft()
        {
            return "{LEFT}";
        }

        public virtual string GetKeyForGamepadDpadRight()
        {
            return "{RIGHT}";
        }

        public virtual string GetKeyForGamepadDpadDown()
        {
            return "{DOWN}";
        }

        public virtual string GetKeyForGamepadDpadUp()
        {
            return "{UP}";
        }
    }
}

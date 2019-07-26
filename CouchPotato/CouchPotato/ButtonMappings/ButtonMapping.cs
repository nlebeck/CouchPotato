using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CouchPotato.ButtonMappings
{
    public interface ButtonMapping
    {
        string GetKeyForGamepadBack();
        string GetKeyForGamepadStart();
        string GetKeyForGamepadDpadLeft();
        string GetKeyForGamepadDpadRight();
        string GetKeyForGamepadDpadUp();
        string GetKeyForGamepadDpadDown();
    }
}

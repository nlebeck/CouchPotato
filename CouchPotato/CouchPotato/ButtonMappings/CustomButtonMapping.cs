using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CouchPotato.ButtonMappings
{
    public class CustomButtonMapping : DefaultButtonMapping
    {
        private string keyForGamepadBack;
        private string keyForGamepadStart;
        private string keyForGamepadDpadLeft;
        private string keyForGamepadDpadRight;
        private string keyForGamepadDpadDown;
        private string keyForGamepadDpadUp;

        public CustomButtonMapping(
            string keyForGamepadBack,
            string keyForGamepadStart,
            string keyForGamepadDpadLeft,
            string keyForGamepadDpadRight,
            string keyForGamepadDpadDown,
            string keyForGamepadDpadUp)
        {
            this.keyForGamepadBack = keyForGamepadBack;
            this.keyForGamepadStart = keyForGamepadStart;
            this.keyForGamepadDpadLeft = keyForGamepadDpadLeft;
            this.keyForGamepadDpadRight = keyForGamepadDpadRight;
            this.keyForGamepadDpadDown = keyForGamepadDpadDown;
            this.keyForGamepadDpadUp = keyForGamepadDpadUp;
        }

        public override string GetKeyForGamepadBack()
        {
            if (keyForGamepadBack == null)
            {
                return base.GetKeyForGamepadBack();
            }
            return keyForGamepadBack;
        }

        public override string GetKeyForGamepadStart()
        {
            if (keyForGamepadStart == null)
            {
                return base.GetKeyForGamepadStart();
            }
            return keyForGamepadStart;
        }

        public override string GetKeyForGamepadDpadLeft()
        {
            if (keyForGamepadDpadLeft == null)
            {
                return base.GetKeyForGamepadDpadLeft();
            }
            return keyForGamepadDpadLeft;
        }

        public override string GetKeyForGamepadDpadRight()
        {
            if (keyForGamepadDpadRight == null)
            {
                return base.GetKeyForGamepadDpadRight();
            }
            return keyForGamepadDpadRight;
        }

        public override string GetKeyForGamepadDpadDown()
        {
            if (keyForGamepadDpadDown == null)
            {
                return base.GetKeyForGamepadDpadDown();
            }
            return keyForGamepadDpadDown;
        }

        public override string GetKeyForGamepadDpadUp()
        {
            if (keyForGamepadDpadUp == null)
            {
                return base.GetKeyForGamepadDpadUp();
            }
            return keyForGamepadDpadUp;
        }
    }
}

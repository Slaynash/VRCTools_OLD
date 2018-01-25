using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRC.Core;

namespace VRCTools
{
    public abstract class DeobfGetters
    {

        public static bool IsVRLaunched()
        {
            return VRCApplicationSetup.FPPKEHEAFCL;
        }

        public static QuickMenu GetQuickMenu_Instance()
        {
            return QuickMenu.GDNHBPCMDKG;
        }
    }
}

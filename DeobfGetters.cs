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

        public static AGOPCCNOLCC getCurrentUser()
        {
            return AGOPCCNOLCC.HAFAJICOBKH;
        }

        public static ApiAvatar getApiAvatar()
        {
            return getCurrentUser().PDOLEEGKCGH;
        }
    }
}

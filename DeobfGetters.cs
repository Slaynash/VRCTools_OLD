using VRC.Core;

namespace VRCTools
{
    public abstract class DeobfGetters
    {

        public static bool IsVRLaunched()
        {
            return VRCApplicationSetup.BKJMAKBMBOI;
        }

        public static QuickMenu GetQuickMenu_Instance()
        {
            return QuickMenu.CPKNOLDHNMK;
        }

        public static CNPCKMCHMPP getCurrentUser()
        {
            return CNPCKMCHMPP.BDJPLJMHAAP;
        }

        public static ApiAvatar getApiAvatar()
        {
            return getCurrentUser().AKEHNNOOMIO;
        }
    }
}

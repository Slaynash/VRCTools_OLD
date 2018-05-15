using System;
using System.Reflection;
using VRC.Core;
using VRC.UI;

namespace VRCTools
{
    public abstract class DeobfGetters
    {

        public static bool IsVRLaunched()
        {
            return VRCTrackingManager.IsInVRMode();
        }

        public static QuickMenu GetQuickMenu_Instance()
        {
            return QuickMenu.DBFNIEMNOKE;
        }
        /*
        public static bool QuickMenu_visible()
        {
            Object r = typeof(QuickMenu).GetField("INCKMBKPDGE", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GetQuickMenu_Instance());
            if (r == null) return false;
            return (bool)r;
        }

        public static bool QuickMenu_rightHand()
        {
            Object r = typeof(QuickMenu).GetField("FOAMDBLKDOL", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GetQuickMenu_Instance());
            if (r == null) return false;
            return (bool)r;
        }

        public static UnityEngine.Vector3 QuickMenu_hmdMenuPositionL()
        {
            return (UnityEngine.Vector3) typeof(QuickMenu).GetField("AELKLFHFOLB", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GetQuickMenu_Instance());
        }

        public static UnityEngine.Vector3 QuickMenu_hmdMenuPositionR()
        {
            return (UnityEngine.Vector3) typeof(QuickMenu).GetField("EOKGMCOCMJI", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GetQuickMenu_Instance());
        }

        public static UnityEngine.Vector3 QuickMenu_hmdMenuRotationL()
        {
            return (UnityEngine.Vector3)typeof(QuickMenu).GetField("EDAJOGOEFGB", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GetQuickMenu_Instance());
        }

        public static UnityEngine.Vector3 QuickMenu_hmdMenuRotationR()
        {
            return (UnityEngine.Vector3)typeof(QuickMenu).GetField("PBBIKKKGNOE", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GetQuickMenu_Instance());
        }

        public static bool QuickMenu_hmd()
        {
            Object r = typeof(QuickMenu).GetField("KGGOGOMMJPL", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GetQuickMenu_Instance());
            if (r == null) return false;
            return (bool)r;
        }

        public static bool inMainMenu()
        {
            Object r = typeof(QuickMenu).GetField("ACADABDAMJG", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GetQuickMenu_Instance());
            if (r == null) return false;
            return (bool)r;
        }
        */
        public static DHCFLIKLEFK getCurrentUser()
        {
            return DHCFLIKLEFK.DLAPCGCBOPJ;
        }

        public static ApiAvatar getApiAvatar()
        {
            return getCurrentUser().PIJCBKHIKKK;
        }
        /*
        internal static UiAvatarList[] GetAvatarLists(PageAvatar pageAvatar)
        {
            FieldInfo field = typeof(PageAvatar).GetField("IDPPBNCJENI", BindingFlags.NonPublic | BindingFlags.Instance);
            if(field == null)
            {
                VRCToolsLogger.Error("AvatarList field is null !");
                return null;
            }
            return field.GetValue(pageAvatar) as UiAvatarList[];
        }

        internal static void SetAvatarLists(PageAvatar pageAvatar, UiAvatarList[] tmpList)
        {
            typeof(PageAvatar).GetField("IDPPBNCJENI", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(pageAvatar, tmpList);
        }
        */
    }
}

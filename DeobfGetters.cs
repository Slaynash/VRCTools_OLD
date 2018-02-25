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
            return QuickMenu.MAPKPPNKIDJ;
        }

        public static CFKHOIICONP getCurrentUser()
        {
            return CFKHOIICONP.PCKIBIBJEMO;
        }

        public static ApiAvatar getApiAvatar()
        {
            return getCurrentUser().IEPDIGCICNL;
        }

        internal static UiAvatarList[] GetAvatarLists(PageAvatar pageAvatar)
        {
            FieldInfo field = typeof(PageAvatar).GetField("IEKGBHLPJNK", BindingFlags.NonPublic | BindingFlags.Instance);
            if(field == null)
            {
                VRCToolsLogger.Error("AvatarList field is null !");
                return null;
            }
            return field.GetValue(pageAvatar) as UiAvatarList[];
        }

        internal static void SetAvatarLists(PageAvatar pageAvatar, UiAvatarList[] tmpList)
        {
            typeof(PageAvatar).GetField("IEKGBHLPJNK", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(pageAvatar, tmpList);
        }
    }
}

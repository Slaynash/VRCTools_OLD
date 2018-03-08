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
            return QuickMenu.FHABPAILCFM;
        }

        public static EKAIFCIHIMG getCurrentUser()
        {
            return EKAIFCIHIMG.GLLFBDBLPMC;
        }

        public static ApiAvatar getApiAvatar()
        {
            return getCurrentUser().ODDGOOOAGHM;
        }

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
    }
}

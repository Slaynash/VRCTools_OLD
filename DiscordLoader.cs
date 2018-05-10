using System.Threading;
using VRC.Core;
using static VRC.Core.ApiWorldInstance;

namespace VRCTools
{
    public abstract class DiscordLoader
    {
        private static DiscordRpc.RichPresence presence;

        public static ApiWorld CurrentWorld { get; private set; }

        public static void Init()
        {
            VRCToolsLogger.Info("[DRPC] Initialising...");
            DiscordRpc.EventHandlers eh = new DiscordRpc.EventHandlers();

            presence.state = "Not in a world";
            presence.partySize = 0;
            presence.partyMax = 0;
            presence.details = "Not logged in" + " (" + (DeobfGetters.IsVRLaunched() ? "VR" : "Desktop") + ")";
            presence.largeImageKey = "logo";

            Thread t = new Thread(new ThreadStart(() => {

                DiscordRpc.Initialize("404400696171954177", ref eh, true, null);

                while (true)
                {
                    Update();
                    Thread.Sleep(5000);
                }
            }));
            t.Name = "Discord-RPC update thread";
            t.IsBackground = true;
            t.Start();

            VRCToolsLogger.Info("[DRPC] Done !");
        }

        public static void Update()
        {
            ApiWorld world = RoomManager.currentRoom;

            if (world != CurrentWorld)
            {
                CurrentWorld = world;
                if (world != null)
                {
                    if (world.currentInstanceAccess == AccessType.InviteOnly || world.currentInstanceAccess == AccessType.InvitePlus)
                        presence.state = "in a private world";
                    else
                        presence.state = "in " + world.name + " #" + world.currentInstanceIdOnly + " " + (
                            world.currentInstanceAccess == AccessType.FriendsOfGuests ? "[Friends+]" :
                            world.currentInstanceAccess == AccessType.FriendsOnly ? "[Friends]" :
                            world.currentInstanceAccess == AccessType.InviteOnly ? "[private]" :
                            world.currentInstanceAccess == AccessType.InvitePlus ? "[private]" :
                            world.currentInstanceAccess == AccessType.Public ? "" :
                            "[Unknown]"
                        );
                }
                else
                {
                    presence.state = "Not in a world";
                }
                if (APIUser.CurrentUser != null)
                {
                    presence.details = "as " + APIUser.CurrentUser.displayName + " (" + (DeobfGetters.IsVRLaunched() ? "VR" : "Desktop") + ")";
                }
                else
                {
                    presence.details = "Not logged in" + " (" + (DeobfGetters.IsVRLaunched() ? "VR" : "Desktop") + ")";
                }
            }

            if(world != null && world.currentInstanceAccess != AccessType.InviteOnly && world.currentInstanceAccess != AccessType.InvitePlus)
            {
                presence.partySize = VRC.PlayerManager.GetAllPlayers().Length;
                presence.partyMax = world.capacity;
            }
            else
            {
                presence.partySize = 0;
                presence.partyMax = 0;
            }

            DiscordRpc.UpdatePresence(ref presence);
        }

    }
}

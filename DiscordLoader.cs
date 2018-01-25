using System;
using System.Threading;
using VRC.Core;
using static DiscordRpc;
using static VRC.Core.ApiWorld.WorldInstance;

namespace VRCTools
{
    public abstract class DiscordLoader
    {
        private static RichPresence presence;

        public static ApiWorld CurrentWorld { get; private set; }

        public static void Init()
        {
            VRCToolsLogger.Info("[DRPC] Initialising...");
            EventHandlers eh = new EventHandlers();

            presence.state = "Not in a world";
            presence.partySize = 0;
            presence.partyMax = 0;
            presence.details = "Not logged in" + " (" + (DeobfGetters.IsVRLaunched() ? "Desktop" : "VR") + ")";
            presence.largeImageKey = "logo";

            Initialize("404400696171954177", ref eh, true, null);
            UpdatePresence(ref presence);

            Thread t = new Thread(new ThreadStart(() => {
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
                    if (world.currentInstanceAccess == AccessType.InviteOnly)
                        presence.state = "in a private world";
                    else
                        presence.state = "in " + world.name + " #" + world.currentInstanceIdOnly + " " + (
                            world.currentInstanceAccess == AccessType.FriendsOfGuests ? "[Friends+]" :
                            world.currentInstanceAccess == AccessType.FriendsOnly ? "[Friends]" :
                            ""
                        );
                }
                else
                {
                    presence.state = "Not in a world";
                }
                if (APIUser.CurrentUser != null)
                {
                    presence.details = "as " + APIUser.CurrentUser.displayName + " (" + (DeobfGetters.IsVRLaunched() ? "Desktop" : "VR") + ")";
                }
                else
                {
                    presence.details = "Not logged in" + " (" + (DeobfGetters.IsVRLaunched() ? "Desktop" : "VR") + ")";
                }
            }

            if(world != null && world.currentInstanceAccess != AccessType.InviteOnly)
            {
                presence.partySize = VRC.PlayerManager.GetAllPlayers().Length;
                presence.partyMax = world.capacity;
            }
            else
            {
                presence.partySize = 0;
                presence.partyMax = 0;
            }

            UpdatePresence(ref presence);
        }

    }
}

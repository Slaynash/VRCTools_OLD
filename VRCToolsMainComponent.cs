using System;
using UnityEngine;

namespace VRCTools {
    public class VRCToolsMainComponent : MonoBehaviour {

        bool discordInit = false;
        bool avatarInit = false;

        public void Awake() {
            VRCToolsLogger.Info("Initialising VRCTools 180130-1928 for Build 501");

            VRCToolsLogger.Init(true);
            try
            {
                DiscordLoader.Init();
                discordInit = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured during the initialisation of DRPC:");
                Console.WriteLine(e);
            }
            try
            {
                AvatarUtils.Init();
                avatarInit = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured during the initialisation of DRPC:");
                Console.WriteLine(e);
            }
            DontDestroyOnLoad(this);

            VRCToolsLogger.Info("Initialised successfully !");
        }

        public void Update() {

            if(discordInit) DiscordLoader.Update();

            if(avatarInit) AvatarUtils.Update();

        }
    }
}

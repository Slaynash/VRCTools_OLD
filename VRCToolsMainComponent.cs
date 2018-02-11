using System;
using System.Collections;
using UnityEngine;

namespace VRCTools {
    public class VRCToolsMainComponent : MonoBehaviour {

        private bool discordInit = false;
        private bool avatarInit = false;
        private bool showUsing = true;

        public static string VRCToolsVersion = "180211-1302";
        public static string GAMEVERSION = "0.12.0p12:507";
        public static string VERSION = VRCToolsVersion + "_" + GAMEVERSION;

        public void Awake() {

            VRCToolsLogger.Info("Initialising VRCTools "+ VRCToolsVersion + " for game version "+ GAMEVERSION);
            VRCTServerManager.Init();
            VRCTServerManager.GetLastestVersion();

            VRCToolsLogger.Init(false);
            VRCToolsLogger.Info("Game download path: " + Application.persistentDataPath);

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
                Console.WriteLine("An error occured during the initialisation of AvatarUtils:");
                Console.WriteLine(e);
            }
            DontDestroyOnLoad(this);

            VRCToolsLogger.Info("Initialised successfully !");

            StartCoroutine(ShowUsing());
        }

        private IEnumerator ShowUsing()
        {
            yield return new WaitForSeconds(8);
            showUsing = false;
        }

        public void Update()
        {
            try
            {
                VRCTServerManager.Update();
                VRCToolsLogger.Update();
                if (discordInit) DiscordLoader.Update();
                if (avatarInit) AvatarUtils.Update();
            }
            catch(Exception e)
            {
                VRCToolsLogger.Error(e.ToString());
            }

        }

        public void OnGUI()
        {
            try {
                int currentPadding = 0;

                currentPadding += VRCTServerManager.OnGUI(currentPadding);
                currentPadding += VRCToolsLogger.OnGUI(currentPadding);

                if (showUsing)
                {
                    GUI.color = Color.green;
                    GUI.Label(new Rect(0, Screen.height - 40 - currentPadding, Screen.width, 20), "Using VRCTools "+VERSION);
                    GUI.Label(new Rect(0, Screen.height - 20 - currentPadding, Screen.width, 20), "Made By Slaynash#2879 (Discord name)");
                    currentPadding += 40;
                }
            }
            catch (Exception e)
            {
                VRCToolsLogger.Error(e.ToString());
            }

        }

        /* Upcoming - DLL Injector
        public static void LoadLibraryA()
        {
            UnityEngine.Debug.Log("DLLInject");
            System.Windows.Forms.MessageBox.Show("You kiddo are going to have a very bad time");
        }
        */
    }
}

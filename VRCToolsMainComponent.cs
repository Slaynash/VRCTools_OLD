using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRCTools {
    public class VRCToolsMainComponent : MonoBehaviour
    {
        public static string VRCToolsVersion = "180212-1306";
        public static string GAMEVERSION = "0.12.0p12:507";
        public static string VERSION = VRCToolsVersion + "_" + GAMEVERSION;

        private static VRCToolsMainComponent instance;

        private bool discordInit = false;
        private bool avatarInit = false;
        
        private static int nbmessagelast = 0;
        private static Dictionary<int, GUIMessage> messagesList = new Dictionary<int, GUIMessage>();

        public void Awake() {
            instance = this;

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

            MessageGUI(Color.green, "Using VRCTools " + VERSION, 8);
            MessageGUI(Color.green, "Made By Slaynash#2879 (Discord name)", 8);
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

        public static void MessageGUI(Color color, string message, int duration)
        {
            int messageId = nbmessagelast++;
            messagesList.Add(messageId, new GUIMessage(message, color));
            instance.MessageGUI_internal(messageId, duration);
        }

        private IEnumerable MessageGUI_internal(int id, int duration)
        {
            yield return new WaitForSeconds(duration);
            messagesList.Remove(id);
        }

        public void OnGUI()
        {
            try {
                int currentPadding = 20;

                currentPadding += VRCTServerManager.OnGUI(currentPadding);
                currentPadding += VRCToolsLogger.OnGUI(currentPadding);

                foreach(KeyValuePair<int, GUIMessage> e in messagesList)
                {
                    GUI.color = e.Value.color;
                    GUI.Label(new Rect(0, Screen.height - currentPadding, Screen.width, 20), e.Value.message);
                    currentPadding += 20;
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
            System.Windows.Forms.MessageBox.Show("You kiddo are going to have a bad time");
        }
        */
    }

    internal class GUIMessage
    {
        public string message;
        public Color color;

        public GUIMessage(string message, Color color)
        {
            this.message = message;
            this.color = color;
        }
    }
}

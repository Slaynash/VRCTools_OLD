using UnityEngine;

namespace VRCTools {
    public class VRCToolsMainComponent : MonoBehaviour {

        public void Awake() {
            VRCToolsLogger.Info("Initialising...");

            VRCToolsLogger.Init(false); //change this to true if you want to see the VRCTools output console
            DiscordLoader.Init();
            DontDestroyOnLoad(this);

            VRCToolsLogger.Info("Initialised successfully !");
        }

        public void Update() {

            DiscordLoader.Update();

        }
    }
}

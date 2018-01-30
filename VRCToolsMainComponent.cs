using UnityEngine;

namespace VRCTools {
    public class VRCToolsMainComponent : MonoBehaviour {

        public void Awake() {
            VRCToolsLogger.Info("Initialising...");

            VRCToolsLogger.Init(true);
            DiscordLoader.Init();
            AvatarUtils.Init();
            DontDestroyOnLoad(this);

            VRCToolsLogger.Info("Initialised successfully !");
        }

        public void Update() {

            DiscordLoader.Update();

            AvatarUtils.Update();

        }
    }
}

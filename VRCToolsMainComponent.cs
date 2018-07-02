using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace VRCTools {
    public class VRCToolsMainComponent : MonoBehaviour
    {
        public static string VRCToolsVersion = "180702-0354";
        public static string GAMEVERSION = "2018.2.2:570";
        public static string VERSION = VRCToolsVersion + "_" + GAMEVERSION;

        private static VRCToolsMainComponent instance;

        private bool discordInit = false;
        private bool avatarInit = false;

        private static int nbmessagelast = 0;
        private static Dictionary<int, GUIMessage> messagesList = new Dictionary<int, GUIMessage>();

        //private GameObject[] cameraHelper = new GameObject[2];

        public void Awake() {
            instance = this;

            VRCToolsLogger.Info("Initialising VRCTools "+ VRCToolsVersion + " for game version "+ GAMEVERSION);
            VRCTServerManager.Init();
            VRCTServerManager.InitConnection();
            VRCTServerManager.GetLastestVersion();

            VRCToolsLogger.Init(false);
            ChangeCacheFolder();
            //VRCToolsLogger.Info("Game download path: " + Application.persistentDataPath);

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
            /*
            try
            {
                InitEnhancedCamera();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured during the initialisation of the Enhanced Camera:");
                Console.WriteLine(e);
            }
            */
            DontDestroyOnLoad(this);

            VRCToolsLogger.Info("Initialised successfully !");

            MessageGUI(Color.green, "Using VRCTools " + VERSION, 8);
            MessageGUI(Color.green, "Made By Slaynash", 8);
            VRCTServerManager.ShowMOTD();
        }

        private void ChangeCacheFolder()
        {
            if (!File.Exists("vrctools_datapath.txt"))
            {
                StreamWriter writer = new StreamWriter("vrctools_datapath.txt", true);
                writer.WriteLine(Application.persistentDataPath);
                writer.Close();
            }

            StreamReader reader = new StreamReader("vrctools_datapath.txt");
            string targetPath = reader.ReadLine();
            reader.Close();
            try
            {
                DirectoryInfo di = Directory.CreateDirectory(targetPath);
                targetPath = di.FullName;
            }
            catch(Exception e)
            {
                VRCToolsLogger.Error(e.ToString());
                VRCToolsLogger.Info("Current cache path: " + Application.persistentDataPath);
                return;
            }

            Func<string> funcPath = () => { return targetPath; };
            BestHTTP.HTTPManager.RootCacheFolderProvider = funcPath;

            Type httpManager = typeof(VRC.Core.ApiAvatar).Assembly.GetType("VRC.Core.BestHTTP.HTTPManager");
            Type httpCacheService = typeof(VRC.Core.ApiAvatar).Assembly.GetType("VRC.Core.BestHTTP.Caching.HTTPCacheService");
            httpManager.GetMethod("set_RootCacheFolderProvider", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Invoke(null, new object[] { funcPath });
            httpCacheService.GetMethod("set_CacheFolder", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).Invoke(null, new object[] { null });
            httpCacheService.GetMethod("CheckSetup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).Invoke(null, new object[] { });

            VRCToolsLogger.Info("Current cache path: " + targetPath);

        }
        // Deprecated as the camera as been modified
        private void InitEnhancedCamera()
        {

            //_Application/TrackingVolume/UserCamera/ViewFinder/PhotoCamera/Cylinder
            //_Application/TrackingVolume/UserCamera/ViewFinder/PhotoCamera/Cylinder (1)
            //cameraHelper[0] = UserCameraController.Instance.viewFinder.transform.Find("PhotoCamera/Cylinder").gameObject;
            //cameraHelper[1] = UserCameraController.Instance.viewFinder.transform.Find("PhotoCamera/Cylinder (1)").gameObject;

            // ZOOM IN
            GameObject zoomInButton = GameObject.CreatePrimitive(PrimitiveType.Cube);
            zoomInButton.transform.SetParent(UserCameraController.Instance.viewFinder.transform);
            zoomInButton.transform.localRotation = Quaternion.identity;
            zoomInButton.transform.localPosition = new Vector3(-0.22f, 0, -0.045f);
            zoomInButton.transform.localScale = new Vector3(0.03f, 0.01f, 0.03f);

            zoomInButton.GetComponent<Collider>().isTrigger = true;
            zoomInButton.GetComponent<Renderer>().material.color = Color.cyan * 0.8f;
            //zoomInButton.layer = cameraHelper[0].layer;

            VRCT_Trigger zoomInTrigger = VRCT_Trigger.CreateVRCT_Trigger(zoomInButton, () => {
                Camera cam1 = UserCameraController.Instance.photoCamera.GetComponent<Camera>();
                if (cam1.fieldOfView - 10 > 0) cam1.fieldOfView -= 10;
                Camera cam2 = UserCameraController.Instance.videoCamera.GetComponent<Camera>();
                if (cam2.fieldOfView - 10 > 0) cam2.fieldOfView -= 10;
                UserCameraController.Instance.speaker.PlayOneShot(UserCameraController.Instance.buttonSound);
            });

            zoomInTrigger.interactText = "Zoom in";
            zoomInTrigger.proximity = 0.4f;


            // ZOOM OUT
            GameObject zoomOutButton = GameObject.CreatePrimitive(PrimitiveType.Cube);
            zoomOutButton.transform.SetParent(UserCameraController.Instance.viewFinder.transform);
            zoomOutButton.transform.localRotation = Quaternion.identity;
            zoomOutButton.transform.localPosition = new Vector3(-0.22f, 0, 0);
            zoomOutButton.transform.localScale = new Vector3(0.03f, 0.01f, 0.03f);

            zoomOutButton.GetComponent<Collider>().isTrigger = true;
            zoomOutButton.GetComponent<Renderer>().material.color = Color.magenta * 0.8f;
            //zoomOutButton.layer = cameraHelper[0].layer;

            VRCT_Trigger zoomOutTrigger = VRCT_Trigger.CreateVRCT_Trigger(zoomOutButton, () => {
                Camera cam1 = UserCameraController.Instance.photoCamera.GetComponent<Camera>();
                if (cam1.fieldOfView + 10 < 180) cam1.fieldOfView += 10;
                Camera cam2 = UserCameraController.Instance.videoCamera.GetComponent<Camera>();
                if (cam2.fieldOfView + 10 < 180) cam2.fieldOfView += 10;
                UserCameraController.Instance.speaker.PlayOneShot(UserCameraController.Instance.buttonSound);
            });

            zoomOutTrigger.interactText = "Zoom out";
            zoomOutTrigger.proximity = 0.4f;

            // CAMERA HELPER
            GameObject toggleCameraHelperButton = GameObject.CreatePrimitive(PrimitiveType.Cube);
            toggleCameraHelperButton.transform.SetParent(UserCameraController.Instance.viewFinder.transform);
            toggleCameraHelperButton.transform.localRotation = Quaternion.identity;
            toggleCameraHelperButton.transform.localPosition = new Vector3(-0.22f, 0, 0.045f);
            toggleCameraHelperButton.transform.localScale = new Vector3(0.03f, 0.01f, 0.03f);

            toggleCameraHelperButton.GetComponent<Collider>().isTrigger = true;
            toggleCameraHelperButton.GetComponent<Renderer>().material.color = Color.yellow * 0.8f;
            //toggleCameraHelperButton.layer = cameraHelper[0].layer;
            /*
            VRCT_Trigger toggleCameraHelperTrigger = VRCT_Trigger.CreateVRCT_Trigger(toggleCameraHelperButton, () => {
                cameraHelper[0].SetActive(!cameraHelper[0].activeSelf);
                cameraHelper[1].SetActive(!cameraHelper[1].activeSelf);
                UserCameraController.Instance.speaker.PlayOneShot(UserCameraController.Instance.buttonSound);
            });

            toggleCameraHelperTrigger.interactText = "Toggle camera helper";
            toggleCameraHelperTrigger.proximity = 0.4f;
            */
        }
        //*/

        public void Update()
        {
            try
            {
                VRCTServerManager.Update();
                VRCToolsLogger.Update();
                if (discordInit) DiscordLoader.Update();
                if (avatarInit) AvatarUtils.Update();
            }
            catch (Exception e)
            {
                VRCToolsLogger.Error(e.ToString());
            }

        }

        public static void MessageGUI(Color color, string message, int duration)
        {
            int messageId = nbmessagelast++;
            messagesList.Add(messageId, new GUIMessage(message, color));
            instance.StartCoroutine(MessageGUI_internal(messageId, duration));
        }

        private static IEnumerator MessageGUI_internal(int id, int duration)
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

                foreach(KeyValuePair<int, GUIMessage> e in messagesList.Reverse())
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

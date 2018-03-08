using BestHTTP.PlatformSupport.TcpClient.General;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using VRC.Core;

namespace VRCTools
{
    class VRCTServerManager
    {
        private const string SERVER_IP = "vrchat.survival-machines.fr";
        //private const string SERVER_IP = "127.0.0.1"; // DEBUG
        private const int PORT_NO = 26341;

        private static TcpClient client;
        private static Stream nwStream;

        private static bool badVersion = false;
        private static bool hiddenBadVersion = false;

        public static bool Init()
        {
            if (client == null || !client.Connected)
            {
                try
                {
                    client = new TcpClient(SERVER_IP, PORT_NO);
                    if (!client.Connected) return false;
                    nwStream = client.GetStream();
                    Thread.Sleep(100);
                }
                catch (Exception e)
                {
                    VRCToolsLogger.Error(e.ToString());
                    return false;
                }
                return true;
            }
            return true;
        }

        public static List<object> GetAvatars()
        {
            List<object> avatars = new List<object>();

            VRCToolsLogger.Info("getAvatars");
            
            if(!Init()) return avatars;

            VRCTRequest request = new VRCTRequest("GET", "");
            Send(request.AsJson());
            String received = Receive();
            VRCToolsLogger.Info(received);

            VRCTResponse response = JsonUtility.FromJson<VRCTResponse>(received);
            if(response.returncode != ReturnCodes.SUCCESS)
            {
                VRCToolsLogger.Error("Unable to get avatars: error code " + response.returncode);
                return avatars;
            }

            VRCToolsLogger.Info(response.data);
            List<SerializableApiAvatar> serializedAvatars = SerializableApiAvatar.ParseJson(response.data);
            foreach(SerializableApiAvatar serializedAvatar in serializedAvatars)
            {
                avatars.Add(serializedAvatar.getDictionary());
            }

            return avatars;
        }

        internal static void ShowMOTD()
        {
            if (!Init()) return;

            VRCTRequest request = new VRCTRequest("GETMOTD", "");
            Send(request.AsJson());
            String received = Receive();
            VRCToolsLogger.Info(received);

            VRCTResponse response = JsonUtility.FromJson<VRCTResponse>(received);
            if (response.returncode == ReturnCodes.SUCCESS)
            {
                string[] motdLines = response.data.Split(new string[]{"<br />"}, StringSplitOptions.None);
                foreach(string line in motdLines){
                    VRCToolsMainComponent.MessageGUI(Color.white, line, 20);
                }
            }

        }

        private static void Send(String text)
        {
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(text+"\n");
            nwStream.Write(bytesToSend, 0, bytesToSend.Length);
            nwStream.Flush();
            VRCToolsLogger.Info(">>> " + text);
        }

        private static string Receive()
        {
            String r = "";
            while (true)
            {
                byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
                string text = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                r += text;
                if (text.EndsWith("\n")) break;
                else VRCToolsLogger.Info("response not ending with \\n, continuing reception...");
            }
            VRCToolsLogger.Info("<<< " + r);
            return r;
        }

        public static int AddAvatar(ApiAvatar apiAvatar)
        {
            if (!Init()) return -1;

            SerializableApiAvatar avatar = new SerializableApiAvatar(
                apiAvatar.id,
                apiAvatar.name,
                apiAvatar.imageUrl,
                apiAvatar.authorName,
                apiAvatar.authorId,
                apiAvatar.assetUrl,
                apiAvatar.description,
                apiAvatar.tags,
                apiAvatar.version,
                apiAvatar.unityPackageUrl,
                apiAvatar.thumbnailImageUrl
            );

            Send(new VRCTRequest("ADD", avatar.AsJson()).AsJson());
            VRCTResponse response = JsonUtility.FromJson<VRCTResponse>(Receive());
            if (response.returncode != ReturnCodes.SUCCESS)
            {
                VRCToolsLogger.Error("Unable to add avatar to favorites: error " + response.returncode);
                return response.returncode;
            }
            VRCToolsLogger.Info("Avatar added to favorites sucessfully");
            return ReturnCodes.SUCCESS;
        }

        public static string GetLastestVersion()
        {
            if (!Init()) return VRCToolsMainComponent.VERSION;

            VRCTRequest request = new VRCTRequest("GETVERSION", "");
            Send(request.AsJson());
            String received = Receive();
            VRCToolsLogger.Info(received);

            VRCTResponse response = JsonUtility.FromJson<VRCTResponse>(received);
            if (response.returncode != ReturnCodes.WAITING_FOR_UPDATE && response.data != VRCToolsMainComponent.VERSION)
            {
                VRCToolsLogger.Warn("Using older version: " + VRCToolsMainComponent.VERSION + " / " + response.data);
                badVersion = true;
                //System.Windows.Forms.MessageBox.Show("Warning: you are not using the lastest version of the mod. Please update to avoid weird bugs");
            }
            return response.data;
        }

        public static void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.L))
            {
                hiddenBadVersion = !hiddenBadVersion;
            }
        }

        public static int OnGUI(int padding)
        {
            if (badVersion && !hiddenBadVersion)
            {
                GUI.color = Color.yellow;
                GUI.Label(new Rect(0, Screen.height - 20 - padding, Screen.width, 20), "VRCTools: Update available (Press CTRL+L to hide/show)");
                GUI.Label(new Rect(0, Screen.height - padding, Screen.width, 20), "VRCTools: Download updater at https://vrchat.survival-machines.fr/vrctools_updater.jar");
                return 40;
            }
            return 0;
        }
    }
}

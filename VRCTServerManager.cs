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

        private static object requestLocker = new object();

        public static void Init()
        {
            Thread th = new Thread(() => {
                while (true)
                {
                    Thread.Sleep(2 * 60 * 1000);
                    if (client == null || !client.Connected) continue;

                    VRCTRequest request = new VRCTRequest("KEEPALIVE", "");
                    RequestSync(request.AsJson());
                }
            });
            th.IsBackground = true;
            th.Name = "VRCTools keepalive";
            th.Start();
        }

        public static bool InitConnection()
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
        /*
        private static void Send(String request)
        {
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(request + "\n");
            nwStream.Write(bytesToSend, 0, bytesToSend.Length);
            nwStream.Flush();
            VRCToolsLogger.Info(">>> " + request);
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
        */
        private static VRCTResponse RequestSync(String request)
        {
            lock (requestLocker)
            {
                //Send(request);
                byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(request + "\n");
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);
                nwStream.Flush();
                VRCToolsLogger.Info(">>> " + request);


                //VRCTResponse response = JsonUtility.FromJson<VRCTResponse>(Receive());
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
                VRCTResponse response = JsonUtility.FromJson<VRCTResponse>(r);


                if (response.returncode == ReturnCodes.BANNED_ACCOUNT)
                {
                    VRCToolsLogger.Warn("Request rejected: Account banned (" + response.data + ")");
                    VRCToolsMainComponent.MessageGUI(Color.red, "Request rejected: Account banned (" + response.data + ")", 3);
                }
                else if (response.returncode == ReturnCodes.BANNED_ADDRESS)
                {
                    VRCToolsLogger.Warn("Request rejected: Address banned (" + response.data + ")");
                    VRCToolsMainComponent.MessageGUI(Color.red, "Request rejected: Address banned (" + response.data + ")", 3);
                }
                return response;
            }
        }











        public static List<object> GetAvatars()
        {
            List<object> avatars = new List<object>();

            VRCToolsLogger.Info("getAvatars");
            
            if(!InitConnection()) return avatars;

            VRCTRequest request = new VRCTRequest("GET", "");

            VRCTResponse response = RequestSync(request.AsJson());
            if (response.returncode != ReturnCodes.SUCCESS)
            {
                if (response.returncode != ReturnCodes.BANNED_ACCOUNT && response.returncode != ReturnCodes.BANNED_ADDRESS)
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
            if (!InitConnection()) return;

            VRCTRequest request = new VRCTRequest("GETMOTD", "");

            VRCTResponse response = RequestSync(request.AsJson());
            if (response.returncode == ReturnCodes.SUCCESS)
            {
                string[] motdLines = response.data.Split(new string[]{"<br />"}, StringSplitOptions.None);
                foreach(string line in motdLines){
                    VRCToolsMainComponent.MessageGUI(Color.white, line, 20);
                }
            }

        }

        public static int AddAvatar(ApiAvatar apiAvatar)
        {
            if (!InitConnection()) return -1;

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

            
            VRCTResponse response = RequestSync(new VRCTRequest("ADD", avatar.AsJson()).AsJson());
            if (response.returncode != ReturnCodes.SUCCESS && response.returncode != ReturnCodes.AVATAR_ALREADY_IN_FAV)
            {
                if(response.returncode != ReturnCodes.BANNED_ADDRESS && response.returncode != ReturnCodes.BANNED_ACCOUNT)
                    VRCToolsLogger.Error("Unable to add avatar to favorites: error " + response.returncode);
                return response.returncode;
            }
            VRCToolsLogger.Info("Avatar added to favorites sucessfully");
            return ReturnCodes.SUCCESS;
        }

        public static string GetLastestVersion()
        {
            if (!InitConnection()) return VRCToolsMainComponent.VERSION;

            VRCTRequest request = new VRCTRequest("GETVERSION", "");

            VRCTResponse response = RequestSync(request.AsJson());
            if (response.returncode != ReturnCodes.WAITING_FOR_UPDATE && response.data != VRCToolsMainComponent.VERSION)
            {
                if (response.returncode != ReturnCodes.BANNED_ADDRESS && response.returncode != ReturnCodes.BANNED_ACCOUNT)
                {
                    VRCToolsLogger.Warn("Using older version: " + VRCToolsMainComponent.VERSION + " / " + response.data);
                    badVersion = true;
                }
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

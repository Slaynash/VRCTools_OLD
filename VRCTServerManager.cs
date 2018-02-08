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
        private const string SERVER_IP = "127.0.0.1";
        private const int PORT_NO = 26341;

        private static TcpClient client;
        private static Stream nwStream;

        private static bool Init()
        {
            client = new TcpClient(SERVER_IP, PORT_NO);
            //client.ReceiveTimeout = 1000;
            if (!client.Connected) return false;
            nwStream = client.GetStream();
            Thread.Sleep(100);
            return true;
        }

        public static List<object> GetAvatars()
        {
            List<object> avatars = new List<object>();

            VRCToolsLogger.Info("getAvatars");
            if (client == null || !client.Connected)
                if(!Init()) return avatars;

            VRCTRequest request = new VRCTRequest("GET", "");
            VRCToolsLogger.Info("Sending...");
            Send(request.AsJson());
            VRCToolsLogger.Info("receiving...");
            String received = Receive();
            VRCToolsLogger.Info(received);

            VRCTResponse response = JsonUtility.FromJson<VRCTResponse>(received);
            if(response.returncode != (int)ReturnCodes.SUCCESS)
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

            VRCToolsLogger.Info("done !");

            //1. request
            //2. parse response to VRCTResponse
            //3. parse response data to list of avatars
            //4. parse avatars to Dictionary<string, object>

            //JsonUtility.FromJson<VRCTResponse>(response)
            //JsonUtility.FromJson<SerializableApiAvatar>(avatars);

            return avatars;
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
            byte[] bytesToRead = new byte[client.ReceiveBufferSize];
            int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
            string text = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
            VRCToolsLogger.Info("<<< " + text);
            return text;
        }

        internal static bool AddAvatar(ApiAvatar apiAvatar)
        {
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
            VRCToolsLogger.Info("sent !");
            VRCTResponse response = JsonUtility.FromJson<VRCTResponse>(Receive());
            if (response.returncode != ReturnCodes.SUCCESS)
            {
                VRCToolsLogger.Error("Unable to add avatar to favorites: error " + response.returncode);
                return false;
            }
            VRCToolsLogger.Info("Avatar added to favorites sucessfully");
            return true;
        }
    }
}

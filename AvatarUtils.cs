using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.Core.BestHTTP;
using VRC.UI;

namespace VRCTools
{
    public abstract class AvatarUtils
    {
        private static List<Action> cb = new List<Action>();

        public static void Init() {
            
        }

        public static void Update()
        {
            try
            {
                lock (cb)
                {
                    foreach(Action a in cb)
                    {
                        a();
                    }
                    cb.Clear();
                }

                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.O))
                {

                    if (PlayerManager.GetCurrentPlayer() == null)
                    {
                        VRCToolsLogger.Info("Unable to get current player");
                        VRCToolsMainComponent.MessageGUI(Color.red, "Unable to get current player", 3);
                    }
                    else
                    {
                        VRCPlayer vrcPlayer1 = PlayerManager.GetCurrentPlayer().vrcPlayer;

                        ApiAvatar apiAvatar1 = DeobfGetters.getApiAvatar();
                        if (apiAvatar1 == null)
                        {
                            VRCToolsLogger.Error("Your avatar couldn't be retrieved. Maybe your Assembly-CSharp.dll is in the wrong version ?");
                            return;
                        }
                        Boolean f = false;
                        if (apiAvatar1.releaseStatus != "public")
                        {
                            VRCToolsMainComponent.MessageGUI(Color.red, "Couldn't add avatar to list: This avatar is not public ! (" + apiAvatar1.name+")", 3);
                        }
                        foreach (String s in apiAvatar1.tags) if (s == "favorite") { f = true; break; }
                        if (!f)
                        {
                            VRCToolsLogger.Info("Adding avatar to favorite: " + apiAvatar1.name);
                            VRCToolsLogger.Info("Description: " + apiAvatar1.description);

                            int rc = VRCTServerManager.AddAvatar(apiAvatar1);
                            if (rc == ReturnCodes.SUCCESS)
                            {
                                apiAvatar1.tags.Add("favorite");
                                VRCToolsMainComponent.MessageGUI(Color.green, "Successfully favorited avatar " + apiAvatar1.name, 3);
                            }
                            else if (rc == ReturnCodes.AVATAR_ALREADY_IN_FAV)
                            {
                                apiAvatar1.tags.Add("favorite");
                                VRCToolsMainComponent.MessageGUI(Color.yellow, "Already in favorite list: " + apiAvatar1.name, 3);
                            }
                            else if (rc == ReturnCodes.AVATAR_PRIVATE)
                            {
                                apiAvatar1.tags.Add("favorite");
                                VRCToolsMainComponent.MessageGUI(Color.red, "Couldn't add avatar to list: This avatar is not public ! (" + apiAvatar1.name + ")", 3);
                            }
                            else VRCToolsMainComponent.MessageGUI(Color.red, "Unable to favorite avatar (error " + rc + "): " + apiAvatar1.name, 3);
                        }
                        else
                        {
                            VRCToolsLogger.Info("This avatar is already in favorite list");
                            VRCToolsMainComponent.MessageGUI(Color.yellow, "Already in favorite list: " + apiAvatar1.name, 3);
                        }
                    }
                }
            }
            catch (Exception e) {
                VRCToolsLogger.Error(e.ToString());
            }
        }

        public static void FetchFavList(string endpoint, HTTPMethods method, Dictionary<string, string> requestParams, Action<List<object>> successCallback, Action<string> errorCallback, bool needsAPIKey, bool authenticationRequired, float cacheLifetime)
        {
            MethodInfo m = typeof(ApiModel).GetMethod(
                "SendRequest",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new Type[] {typeof(string), typeof(HTTPMethods), typeof(Dictionary<string, string>), typeof(Action<string>), typeof(Action<Dictionary<string, object>>), typeof(Action<List<object>>), typeof(Action<string>), typeof(bool), typeof(bool), typeof(float) },
                null
            );

            Action<List<object>> sc = new Action<List<object>>((list) => {
                Thread t = new Thread(new ThreadStart(() => {
                    list.AddRange(VRCTServerManager.GetAvatars());

                    lock (cb)
                    {
                        cb.Add(
                            new Action(() => {
                                successCallback(list);
                            })
                        );
                    }
                    
                }));
                t.Start();
            });
            Action<string> ec = new Action<string>((error) => {
                errorCallback(error);
            });
            
            m.Invoke(null, new object[] { "avatars", HTTPMethods.Get, requestParams, null, null, sc, ec, needsAPIKey, authenticationRequired, cacheLifetime });

        }
        
    }
}
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.Core.BestHTTP;

namespace VRCTools
{
    public abstract class AvatarUtils
    {

        public static void Init()
        {
            
        }

        public static void Update()
        {
            try
            {
                if (PlayerManager.GetCurrentPlayer() == null) return;
                VRCPlayer vrcPlayer1 = PlayerManager.GetCurrentPlayer().vrcPlayer;

                if (DeobfGetters.GetQuickMenu_Instance() == null) return;

                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.O))
                {
                    ApiAvatar apiAvatar1 = DeobfGetters.getApiAvatar();
                    if (apiAvatar1 == null)
                    {
                        VRCToolsLogger.Error("Your avatar couldn't be retrieved. Maybe your Assembly-CSharp.dll is in the wrong version ?");
                        return;
                    }
                    ApiAvatar av = new ApiAvatar();
                    Boolean f = false;
                    av.Init(DeobfGetters.getCurrentUser(), apiAvatar1.name, apiAvatar1.imageUrl, apiAvatar1.assetUrl, apiAvatar1.description, apiAvatar1.tags, apiAvatar1.unityPackageUrl);
                    foreach (String s in av.tags) if (s == "favorite") { f = true; break; }
                    if (!f)
                    {
                        VRCToolsLogger.Info("Adding avatar to favorite: " + apiAvatar1.name);
                        VRCToolsLogger.Info("Description: " + apiAvatar1.description);

                        apiAvatar1.tags.Add("favorite");
                        av.tags.Add("favorite");
                        av.Save(
                            succes => VRCToolsLogger.Info("Avatar added to favorites succesfully"),
                            error => VRCToolsLogger.Info("Unable to add avatar to favorites: " + error)
                        );
                        
                    }
                    else
                    {
                        VRCToolsLogger.Info("This avatar is already in favorite list");
                    }
                }
                /*
                if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.I))
                {
                    ApiAvatar apiAvatar1 = DeobfGetters.getApiAvatar();
                    if (apiAvatar1 == null)
                    {
                        VRCToolsLogger.Error("Your avatar couldn't be retrieved. Maybe your Assembly-CSharp.dll is in the wrong version ?");
                        return;
                    }
                    Console.WriteLine("Avatar informations :");
                    Console.WriteLine("Avatar id: {0}", apiAvatar1.id);
                    Console.WriteLine("Avatar name: {0}", apiAvatar1.name);
                    Console.WriteLine("Avatar image url: {0}", apiAvatar1.imageUrl);
                    Console.WriteLine("Avatar author name: {0}", apiAvatar1.authorName);
                    Console.WriteLine("Avatar author id: {0}", apiAvatar1.authorId);
                    Console.WriteLine("Avatar asset url: {0}", apiAvatar1.assetUrl);
                    Console.WriteLine("Avatar description: {0}", apiAvatar1.description);
                    Console.WriteLine("Avatar tags: {0}", apiAvatar1.tags.Count);
                    foreach (String s in apiAvatar1.tags)
                        Console.WriteLine("- {0} ", s);
                    Console.WriteLine("Avatar version: {0}", apiAvatar1.version);
                    Console.WriteLine("Avatar unity package url: {0}", apiAvatar1.unityPackageUrl);
                    Console.WriteLine("Avatar thumbnail image url: {0}", apiAvatar1.thumbnailImageUrl);
                    Console.WriteLine("Avatar unity version: {0}", apiAvatar1.assetVersion.UnityVersion);
                    Console.WriteLine("Avatar api version: {0}", apiAvatar1.assetVersion.ApiVersion);
                    Console.WriteLine("Avatar platform: {0}", apiAvatar1.platform);
                }
                */
                /*
                if (Input.GetKeyDown(KeyCode.P)) 
                {
                    ApiAvatar apiAvatar1 = DeobfGetters.getApiAvatar();
                    if (apiAvatar1 == null)
                    {
                        VRCToolsLogger.Error("Your avatar couldn't be retrieved. Maybe your Assembly-CSharp.dll is in the wrong version ?");
                        return;
                    }
                    else
                    {
                        if (apiAvatar1.tags.Contains("favorite"))
                        {
                            ApiAvatar.Delete(
                                apiAvatar1.id,
                                () => Console.WriteLine("Avatar removed"),
                                e => Console.WriteLine("Unable to remove avatar from list: "+e)
                            );
                        }
                        else
                        {
                            Console.WriteLine("Unable to remove avatar from list: This is not a favorited avatar");
                        }
                    }
                }
                */

            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }


        public static void FetchFavList(string endpoint, HTTPMethods method, Dictionary<string, string> requestParams, Action<List<object>> successCallback, Action<string> errorCallback, bool needsAPIKey, bool authenticationRequired, float cacheLifetime)
        {
            VRCToolsLogger.Info("FetchFavList v2");

            MethodInfo m = typeof(ApiModel).GetMethod(
                "SendRequest",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new Type[] {typeof(string), typeof(HTTPMethods), typeof(Dictionary<string, string>), typeof(Action<string>), typeof(Action<Dictionary<string, object>>), typeof(Action<List<object>>), typeof(Action<string>), typeof(bool), typeof(bool), typeof(float) },
                null
            );
            Action<List<object>> sc = new Action<List<object>>((list) => {
                /*
                list.ForEach(o => {
                    Dictionary<String, object> dic = o as Dictionary<String, object>;
                    foreach (KeyValuePair<string, object> pair in dic)
                    {
                        VRCToolsLogger.Info(pair.Key+" : "+pair.Value);
                    }
                });
                */
                list.AddRange(VRCTServerManager.getAvatars());
                VRCToolsLogger.Info("SendRequest success");
                successCallback(list);
            });
            Action<string> ec = new Action<string>((error) => {
                VRCToolsLogger.Info("SendRequest error");
                errorCallback(error);
            });
            
            m.Invoke(null, new object[] { "avatars", HTTPMethods.Get, requestParams, null, null, sc, ec, needsAPIKey, authenticationRequired, cacheLifetime });

        }



        /*
        UiAvatarList favList = listGameObject.AddComponent(typeof(UiAvatarList)) as UiAvatarList;
        favList.category = (UiAvatarList.AGDKGNJJFMA)3;
        favList.name = "favoriteList";
        favList.hideWhenEmpty = false;
        */
    }
}
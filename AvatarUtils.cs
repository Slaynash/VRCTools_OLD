using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.Core.BestHTTP;
using VRC.UI;

namespace VRCTools
{
    public abstract class AvatarUtils
    {

        public static void Init() { }

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
                    Boolean f = false;
                    foreach (String s in apiAvatar1.tags) if (s == "favorite") { f = true; break; }
                    if (!f)
                    {
                        VRCToolsLogger.Info("Adding avatar to favorite: " + apiAvatar1.name);
                        VRCToolsLogger.Info("Description: " + apiAvatar1.description);

                        int rc = VRCTServerManager.AddAvatar(apiAvatar1);
                        if(rc == ReturnCodes.SUCCESS)
                        {
                            apiAvatar1.tags.Add("favorite");
                            VRCToolsMainComponent.MessageGUI(Color.green, "Successfully favorited avatar " + apiAvatar1.name, 3);
                        }
                        else if(rc == ReturnCodes.AVATAR_ALREADY_IN_FAV)
                        {
                            apiAvatar1.tags.Add("favorite");
                            VRCToolsMainComponent.MessageGUI(Color.yellow, "Already in favorite list: " + apiAvatar1.name, 3);
                        }
                        else VRCToolsMainComponent.MessageGUI(Color.red, "Unable to favorite avatar (error "+rc+"): " + apiAvatar1.name, 3);
                    }
                    else
                    {
                        VRCToolsLogger.Info("This avatar is already in favorite list");
                        VRCToolsMainComponent.MessageGUI(Color.yellow, "Already in favorite list: " + apiAvatar1.name, 3);
                    }
                }
                /* (debug)
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
                /* upcoming - avatar remove
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

        private static IEnumerable MessageAvatarSave(int output, string message)
        {
            yield return new WaitForSeconds(3);
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
                list.AddRange(VRCTServerManager.GetAvatars());
                successCallback(list);
            });
            Action<string> ec = new Action<string>((error) => {
                errorCallback(error);
            });
            
            m.Invoke(null, new object[] { "avatars", HTTPMethods.Get, requestParams, null, null, sc, ec, needsAPIKey, authenticationRequired, cacheLifetime });

        }
        
        // UPCOMING - Favorite list in the avatar tab
        public static void InjectFavList(PageAvatar pageAvatar) {}
        /* old
        UiAvatarList favList = listGameObject.AddComponent(typeof(UiAvatarList)) as UiAvatarList;
        favList.category = (UiAvatarList.AGDKGNJJFMA)3;
        favList.name = "favoriteList";
        favList.hideWhenEmpty = false;
        */
        /*
        private static bool runned = false;
        public static void InjectFavList(PageAvatar pageAvatar)
        {
            if (!runned)
            {
                Console.WriteLine("HHHHMMMMMMM SOOOO GOOOOOOD !!!!");

                UiAvatarList[] uiAvatarList = DeobfGetters.GetAvatarLists(pageAvatar);

                UiAvatarList[] tmpList = new UiAvatarList[uiAvatarList.Length + 1];
                for (int i = 0; i < uiAvatarList.Length; i++)
                {
                    tmpList[i] = uiAvatarList[i];
                }

                VRCToolsLogger.Info("SimpleAvatarPedestal: " + uiAvatarList[0].avatarPedestal);
                //empty by default //VRCToolsLogger.Info("devPaginatedLists (size): " + uiAvatarList[0].devPaginatedLists.Count);
                VRCToolsLogger.Info("category: " + uiAvatarList[0].category);
                VRCToolsLogger.Info("heading: " + uiAvatarList[0].heading);
                VRCToolsLogger.Info("order: " + uiAvatarList[0].order);
                VRCToolsLogger.Info("scrollRect: " + uiAvatarList[0].scrollRect);
                VRCToolsLogger.Info("scrollRect: " + uiAvatarList[0].scrollRect);



                try
                {
                    tmpList[uiAvatarList.Length] = CreateFavList(pageAvatar);
                    foreach (UiAvatarList a in uiAvatarList) a.myPage = pageAvatar;

                    DeobfGetters.SetAvatarLists(pageAvatar, tmpList);
                }
                catch(Exception e)
                {
                    VRCToolsLogger.Error(e.ToString());
                }
                runned = true;
            }
        }

        private static UiAvatarList CreateFavList(PageAvatar pageAvatar)
        {
            UiAvatarList favList = pageAvatar.gameObject.AddComponent<UiAvatarList>();
            favList.name = "Favorite Avatar List";

            return favList;
        }
        */
    }
}
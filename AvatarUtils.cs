using System;
using System.Collections.Generic;
using UnityEngine;
using VRC;
using VRC.Core;

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

                if (Input.GetKeyDown("O"))
                {
                    ApiAvatar apiAvatar1 = DeobfGetters.getApiAvatar();
                    if (apiAvatar1 == null)
                    {
                        VRCToolsLogger.Error("Your avatar couldn't be retrieved. Maybe your Assembly-CSharp.dll is in the wrong version ?");
                        return;
                    }
                    else
                    {
                        Console.WriteLine("apiAvatar1 {0}:", apiAvatar1);
                        Console.WriteLine("Avatar informations :");
                        Console.WriteLine("Weared by: {0}", vrcPlayer1.name);
                        Console.WriteLine("Avatar name: {0}", apiAvatar1.name);
                        Console.WriteLine("Avatar image url: {0}", apiAvatar1.imageUrl);
                        Console.WriteLine("Avatar asset url: {0}", apiAvatar1.assetUrl);
                        Console.WriteLine("Avatar description: {0}", apiAvatar1.description);
                        Console.WriteLine("Avatar tags: {0}", apiAvatar1.tags.Count);
                        foreach (String s in apiAvatar1.tags)
                            Console.WriteLine("- {0} ", s);
                        Console.WriteLine("Avatar unity package url: {0}", apiAvatar1.unityPackageUrl);
                    }
                    Console.WriteLine("Saving avatar...");
                    ApiAvatar av = new ApiAvatar();
                    Boolean f = false;
                    av.Init(DeobfGetters.getCurrentUser(), apiAvatar1.name, apiAvatar1.imageUrl, apiAvatar1.assetUrl, apiAvatar1.description, apiAvatar1.tags, apiAvatar1.unityPackageUrl);
                    foreach (String s in av.tags) if (s == "favorite") { f = true; break; }
                    if (!f) av.tags.Add("favorite");
                    av.Save(
                        succes => Console.WriteLine("Avatar added to favorites succesfully"),
                        error => Console.WriteLine("Unable to add avatar to favorites: "+error)
                    );
                }
                if (Input.GetKeyDown("P"))
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

            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        
        public static void FetchFavList(Action<List<ApiAvatar>> action)
        {
            VRCToolsLogger.Info("FetchFavList");
            /*
            UiAvatarList favList = listGameObject.AddComponent(typeof(UiAvatarList)) as UiAvatarList;
            favList.category = (UiAvatarList.AGDKGNJJFMA)3;
            favList.name = "favoriteList";
            favList.hideWhenEmpty = false;
            */
        }
    }
}
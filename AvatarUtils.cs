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

                if (Input.GetKeyDown(KeyCode.O))
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
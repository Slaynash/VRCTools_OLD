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

        public static void Init() { }

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
                            VRCToolsMainComponent.MessageGUI(Color.red, "Couldn't add avatar to list: This avatar is not public !" + apiAvatar1.name, 3);
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
                            else VRCToolsMainComponent.MessageGUI(Color.red, "Unable to favorite avatar (error " + rc + "): " + apiAvatar1.name, 3);
                        }
                        else
                        {
                            VRCToolsLogger.Info("This avatar is already in favorite list");
                            VRCToolsMainComponent.MessageGUI(Color.yellow, "Already in favorite list: " + apiAvatar1.name, 3);
                        }
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
                VRCToolsLogger.Error(e.ToString());
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
                VRCToolsLogger.Info("pickerPrefab: " + uiAvatarList[0].pickerPrefab);
                VRCToolsLogger.Info("content: " + uiAvatarList[0].content);
                VRCToolsLogger.Info("transform -> [ViewPort/Content]: " + uiAvatarList[0].transform.Find("ViewPort/Content"));
                VRCToolsLogger.Info("transform -> [ViewPort]: " + uiAvatarList[0].transform.Find("ViewPort"));
                VRCToolsLogger.Info("transform -> [ViewPort] -> [Content]: " + uiAvatarList[0].transform.Find("ViewPort").Find("Content"));
                foreach(Component c in uiAvatarList[0].transform.GetComponentsInChildren<RectTransform>())
                {
                    VRCToolsLogger.Info("comp of type RectTransform: " + c);

                }



                try
                {
                    tmpList[uiAvatarList.Length] = CreateFavList(pageAvatar, uiAvatarList[0]);
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

        private static UiAvatarList CreateFavList(PageAvatar pageAvatar, UiAvatarList fl)
        {
            pageAvatar.gameObject.SetActive(false);
            VRCToolsLogger.Info("STEP 1");
            UiAvatarList favList = pageAvatar.gameObject.AddComponent<UiAvatarList>();

            favList.scrollRect = UnityEngine.Object.Instantiate(fl.scrollRect);

            //*
            ScrollRectEx sre = favList.gameObject.AddComponent<ScrollRectEx>();
            {
                sre.gameObject.name = "Content";
                sre.horizontal = true;
                sre.vertical = false;
                sre.movementType = UnityEngine.UI.ScrollRect.MovementType.Elastic;
                sre.elasticity = 0.1f;
                sre.inertia = true;
                sre.decelerationRate = 0.135f;
                sre.scrollSensitivity = 1.0f;
                VRCToolsLogger.Info("STEP 1.1");
                sre.viewport = new GameObject().AddComponent<RectTransform>();
                    sre.viewport.localPosition = new Vector3(51.682251f, 42.000076f, 0f);
                    sre.viewport.sizeDelta = new Vector2(401.503632f, -57.999695f);
                    sre.viewport.anchorMin = new Vector2(0, 0);
                    sre.viewport.anchorMax = new Vector2(0.738474f, 1);
                    sre.viewport.anchoredPosition = new Vector2(200.751877f, -57.999969f);
                    sre.viewport.localRotation = new Quaternion(0, 0, 0, 1);
                    sre.viewport.localScale = new Vector3(1, 1, 1);
                    sre.viewport.pivot = new Vector2(0.5f, 1f);
                    //sre.viewport.SetParent(favList);
                    VRCToolsLogger.Info("STEP 1.2");
                    RectTransform wpc = sre.viewport.gameObject.AddComponent<RectTransform>();
                        wpc.localPosition = new Vector3(51.682251f, 42.000076f, 0f);
                        wpc.sizeDelta = new Vector2(0, 200);
                        wpc.anchorMin = new Vector2(0, 0);
                        wpc.anchorMax = new Vector2(0.738474f, 1);
                        wpc.anchoredPosition = new Vector2(200.751877f, -57.999969f);
                        wpc.localRotation = new Quaternion(0, 0, 0, 1);
                        wpc.localScale = new Vector3(1, 1, 1);
                        wpc.pivot = new Vector2(0f, 1f);
                        //wpc.SetParent(sre.viewport);

                VRCToolsLogger.Info("STEP 1.3");
                sre.content = wpc;
                favList.content = wpc;
                sre.verticalScrollbarVisibility = UnityEngine.UI.ScrollRect.ScrollbarVisibility.Permanent;
                sre.verticalScrollbarSpacing = 0f;
                sre.horizontalScrollbarVisibility = UnityEngine.UI.ScrollRect.ScrollbarVisibility.Permanent;
                sre.horizontalScrollbarSpacing = 0f;
                VRCToolsLogger.Info("STEP 1.4");
            }
            * /
            VRCToolsLogger.Info("STEP 2");
            
            favList.gameObject.name = "Favorite Avatar List";

            favList.avatarPedestal = fl.avatarPedestal;
            favList.pickerPrefab = fl.pickerPrefab;
            favList.expandButton = GameObject.Instantiate(fl.expandButton);
            favList.expandButton.onClick.RemoveAllListeners();
            favList.expandButton.onClick.AddListener(() => { favList.ToggleExtend(); });
            favList.expandSprite = fl.expandSprite;
            favList.contractSprite = fl.contractSprite;

            favList.hideWhenEmpty = false;
            VRCToolsLogger.Info("existing recttranform: "+favList.transform.gameObject.GetComponent<RectTransform>().name);
            //RectTransform t = favList.transform.gameObject.AddComponent<RectTransform>();
            RectTransform t = UnityEngine.Object.Instantiate(fl.transform.Find("ViewPort") as RectTransform);
            t.name = "ViewPort";
            //t.gameObject.AddComponent<RectTransform>();
            UnityEngine.Object.Instantiate(fl.transform.Find("ViewPort/Content") as RectTransform);
            //t.name = "Content";

            VRCToolsLogger.Info("STEP 3");
            try
            {
                pageAvatar.gameObject.SetActive(true);
            }
            catch(NullReferenceException e)
            {
                //VRCToolsLogger.Info(favList.spacing.ToString());
                //VRCToolsLogger.Info(favList.grid.ToString());
                //VRCToolsLogger.Info(favList.layoutElement.ToString());
                //VRCToolsLogger.Info(favList.numElementsPerPage.ToString());
                return null;
            }
            VRCToolsLogger.Info("paginatedLists: " + favList.paginatedLists);
            VRCToolsLogger.Info("scrollRect: " + favList.scrollRect);
            VRCToolsLogger.Info("transform -> [ViewPort/Content]: " + favList.transform.Find("ViewPort/Content"));
            VRCToolsLogger.Info("transform -> [ViewPort]: " + favList.transform.Find("ViewPort"));
            VRCToolsLogger.Info("transform -> [ViewPort] -> [Content]: " + favList.transform.Find("ViewPort").Find("Content"));
            VRCToolsLogger.Info("content: " + favList.content);
            VRCToolsLogger.Info("STEP 4");

            return favList;
        }
        */
        
    }
}
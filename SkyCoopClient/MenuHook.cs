using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace SkyCoop
{
    public class MenuHook
    {
        public static string s_CurrenetMenuOverride = "Original";
        public static void AddButton(Panel_MainMenu __instance, string name, int order, int plus)
        {
            Panel_MainMenu.MainMenuItem mainMenuItem = new Panel_MainMenu.MainMenuItem();
            mainMenuItem.m_LabelLocalizationId = name;
            mainMenuItem.m_Type = (Panel_MainMenu.MainMenuItem.MainMenuItemType)30 + plus;
            __instance.m_MenuItems.Insert(order, mainMenuItem);

            string id = __instance.m_MenuItems[order].m_Type.ToString();
            int type = (int)__instance.m_MenuItems[order].m_Type;

            __instance.m_BasicMenu.AddItem(id, type, order, name, "", "", new Action(__instance.OnSandbox), Color.gray, Color.white);
        }

        public static void OverrideMenuButton(Transform Grid, int index, string txt, bool onClickHack = true)
        {
            UILabel Label = Grid.GetChild(index).GetChild(0).GetComponent<UILabel>();
            UIButton Button = Grid.GetChild(index).GetComponent<UIButton>();

            if (onClickHack)
            {
                if (Grid.GetChild(index).gameObject.GetComponent<Comps.UiButtonKeyboardPressSkip>() == null)
                {
                    Comps.UiButtonKeyboardPressSkip Skip = Grid.GetChild(index).gameObject.AddComponent<Comps.UiButtonKeyboardPressSkip>();
                    Skip.m_Click = Button.onClick;
                    Button.onClick = null;
                } else if (Grid.GetChild(index).gameObject.GetComponent<Comps.UiButtonKeyboardPressSkip>() != null)
                {
                    Comps.UiButtonKeyboardPressSkip Skip = Grid.GetChild(index).gameObject.GetComponent<Comps.UiButtonKeyboardPressSkip>();
                    Skip.m_Click = Button.onClick;
                    Button.onClick = null;
                }
            }

            Label.mText = txt;
            Label.text = txt;
            Label.ProcessText();
            Grid.GetChild(index).gameObject.SetActive(true);
        }

        public static void ReturnOriginalButtons(Transform Grid, int index)
        {
            UIButton Button = Grid.GetChild(index).GetComponent<UIButton>();
            if (Grid.GetChild(index).gameObject.GetComponent<Comps.UiButtonKeyboardPressSkip>() != null)
            {
                Comps.UiButtonKeyboardPressSkip Skip = Grid.GetChild(index).gameObject.GetComponent<Comps.UiButtonKeyboardPressSkip>();
                Button.onClick = Skip.m_Click;
            }
        }

        public static void ClearMenuButtons(Transform Grid)
        {
            for (int i = 0; i <= 6; i++)
            {
                Grid.GetChild(i).gameObject.SetActive(false);
            }
        }

        public static void ChangeMenuItems(string mode)
        {
            Panel_Sandbox Panel = InterfaceManager.GetPanel<Panel_Sandbox>();

            s_CurrenetMenuOverride = mode;

            Logger.Log("[UI] ChangeMenuItems s_CurrenetMenuOverride " + s_CurrenetMenuOverride);

            //                                                   RootMenu    Menu        Left_Align  Grid
            Transform Grid = Panel.gameObject.transform.GetChild(0).GetChild(0).GetChild(6).GetChild(3);

            if (mode == "Original")
            {
                ReturnOriginalButtons(Grid, 0);
                ReturnOriginalButtons(Grid, 1);
                ReturnOriginalButtons(Grid, 2);
                ReturnOriginalButtons(Grid, 3);
                Panel.gameObject.transform.GetChild(3).GetChild(2).gameObject.SetActive(true);//SurvivalTitle_Texture
                return;
            }

            InterfaceManager.TrySetPanelEnabled<Panel_Sandbox>(true);

            Panel.gameObject.transform.GetChild(3).GetChild(2).gameObject.SetActive(false);//SurvivalTitle_Texture
            ClearMenuButtons(Grid);

            if (mode == "Multiplayer")
            {
                Logger.Log("[UI] Multiplayer build");
                OverrideMenuButton(Grid, 1, "HOST SERVER");
                OverrideMenuButton(Grid, 2, "JOIN SERVER");
                OverrideMenuButton(Grid, 3, "SETTINGS");
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(BasicMenu), "UpdateDescription", null)]
        internal class BasicMenu_UpdateDescription_Post
        {
            public static void Postfix(BasicMenu __instance, int buttonIndex)
            {
                if (__instance == null || buttonIndex >= __instance.m_ItemModelList.Count)
                {
                    return;
                }
                BasicMenu.BasicMenuItemModel Button = __instance.m_ItemModelList[buttonIndex];
                if (Button == null)
                {
                    return;
                }

                if(s_CurrenetMenuOverride == "" || s_CurrenetMenuOverride == "Original")
                {
                    if (Button.m_DescriptionText == "GAMEPLAY_Description31")
                    {
                        __instance.m_DescriptionLabel.text = "Host or join a multiplayer session.";
                    }
                }else if (s_CurrenetMenuOverride == "Multiplayer")
                {
                    if (Button.m_Id == "GAMEPLAY_Resume")
                    {
                        __instance.m_DescriptionLabel.text = "Configure and host a session.";
                    } else if (Button.m_Id == "GAMEPLAY_Load")
                    {
                        //if (SteamConnect.CanUseSteam)
                        //{
                        //    __instance.m_DescriptionLabel.text = "Find a server or join by IP address.";
                        //} else
                        //{
                            __instance.m_DescriptionLabel.text = "Join by IP address.";
                        //}
                    } else if (Button.m_Id == "GAMEPLAY_Challenges")
                    {
                        __instance.m_DescriptionLabel.text = "Change your nickname and customize your appeal.";
                    }
                }else if (s_CurrenetMenuOverride == "Join")
                {
                    if (Button.m_Id == "GAMEPLAY_Resume")
                    {
                        __instance.m_DescriptionLabel.text = "Browse public servers.";
                    } else if (Button.m_Id == "GAMEPLAY_Load")
                    {
                        __instance.m_DescriptionLabel.text = "Connect to a server by IP address.";
                    } else if (Button.m_Id == "GAMEPLAY_Challenges")
                    {
                        __instance.m_DescriptionLabel.text = "Opens steam friends overlay.";
                    }
                }else if (s_CurrenetMenuOverride == "MultiProfileSettings")
                {
                    if (Button.m_Id == "GAMEPLAY_Resume")
                    {
                        __instance.m_DescriptionLabel.text = "Change your character name. Other players will see it ingame.";
                    } else if (Button.m_Id == "GAMEPLAY_Load")
                    {
                        __instance.m_DescriptionLabel.text = "Toggle supporter bonuses and select flairs.";
                    } else if (Button.m_Id == "GAMEPLAY_Challenges")
                    {
                        __instance.m_DescriptionLabel.text = "Copy to clipboard EGS or Steam Account ID.";
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "Initialize", null)]
        public class Panel_MainMenu_Start
        {
            public static void Postfix(Panel_MainMenu __instance)
            {
                Logger.Log("[UI] Trying modify main menu...");
                AddButton(__instance, "MULTIPLAYER", 3, 1);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(UIButton), "OnClick")]
        internal class UIButton_Press
        {
            private static bool Prefix(UIButton __instance)
            {
                Logger.Log("UIButton OnClick");
                if (__instance.gameObject != null && __instance.gameObject.GetComponent<Comps.UiButtonPressHook>() != null)
                {
                    Comps.UiButtonPressHook Hook = __instance.gameObject.GetComponent<Comps.UiButtonPressHook>();
                    Logger.Log("Clicked m_CustomId " + Hook.m_CustomId);
                    Logger.Log("Clicked m_PanelHandle " + Hook.m_PanelHandle);

                    if(Hook.m_PanelHandle == "Panel_MainMenu" && Hook.m_CustomId == 3) // MULTIPLAYER MAIN MENU
                    {
                        ChangeMenuItems("Multiplayer");
                        InterfaceManager.TrySetPanelEnabled<Panel_MainMenu>(false);
                    }
                }
                //ModMain.Server.GameServer.StartServer(1111, 4);
                //Thread.Sleep(15);
                //ModMain.Client.ConnectToServer("localhost", 1111);

                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Sandbox), "OnClickBack", null)]
        public class Panel_Sandbox_OnClickBack
        {
            public static bool Prefix(Panel_Sandbox __instance)
            {
                if (s_CurrenetMenuOverride == "Multiplayer")
                {
                    ChangeMenuItems("Original");
                    InterfaceManager.TrySetPanelEnabled<Panel_Sandbox>(false);
                    InterfaceManager.TrySetPanelEnabled<Panel_MainMenu>(true);
                    return true;
                }

                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "Update", null)]
        public class Panel_MainMenu_Update
        {
            public static void Postfix(Panel_MainMenu __instance)
            {
                // MainPanel/MenuRoot/Menu/Left_Align/Grid
                // 0        /0       /0   /6         /3
                Transform Grid = __instance.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(3);
                for (int i = 0; i <= 6; i++)
                {
                    GameObject Button = Grid.GetChild(i).gameObject;

                    if (Button.GetComponent<UIButton>() != null)
                    {
                        if (Button.GetComponent<Comps.UiButtonPressHook>() == null)
                        {
                            Button.AddComponent<Comps.UiButtonPressHook>();
                            Button.GetComponent<Comps.UiButtonPressHook>().m_CustomId = i;
                            Button.GetComponent<Comps.UiButtonPressHook>().m_PanelHandle = __instance.GetType().Name;
                        }
                    }
                }
            }
        }
    }
}

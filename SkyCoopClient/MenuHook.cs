using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace SkyCoop
{
    public class MenuHook
    {
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

                if (Button.m_DescriptionText == "GAMEPLAY_Description31")
                {
                    __instance.m_DescriptionLabel.text = "Host or join a multiplayer session.";
                }
            }

        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "Initialize", null)]
        public class Panel_MainMenu_Start
        {
            public static void Postfix(Panel_MainMenu __instance)
            {
                MelonLogger.Msg("[UI] Trying modify main menu...");
                AddButton(__instance, "MULTIPLAYER", 4, 1);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(UIButton), "OnClick")]
        internal class UIButton_Press
        {
            private static bool Prefix(UIButton __instance)
            {
                //if (__instance.gameObject != null && __instance.gameObject.GetComponent<Comps.UiButtonPressHook>() != null)
                //{
                //    int CustomId = __instance.gameObject.GetComponent<Comps.UiButtonPressHook>().m_CustomId;
                //    MelonLogger.Msg("Clicked m_CustomId " + CustomId);
                //}

                ModMain.Server.GameServer.StartServer(1111, 4);
                Thread.Sleep(15);
                ModMain.Client.ConnectToServer("localhost", 1111);

                return true;
            }
        }

    }
}

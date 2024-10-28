﻿using Il2Cpp;
using Il2CppTLD.Gameplay;
using Il2CppTLD.Scenes;
using MelonLoader;
using SkyCoopServer;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace SkyCoop;

internal sealed class ModMain : MelonMod
{
    public static Server Server;
    public static Client Client;

    public override void OnInitializeMelon()
    {
        Server = new Server();
        Client = new Client();
    }

    public static void SetAppBackgroundMode()
    {
        if (Application.runInBackground == false) Application.runInBackground = true; // Always running in bg
    }

    [Obsolete]
    public override void OnApplicationStart()
    {
        Comps.RegisterComponents();
        AssetManager.PreloadMainBundle();
        //AssetManager.DumpPrefabsList();
    }

    public static void OnGameBoot()
    {
        ReimplementConsole();
        //AssetManager.DumpAddressablesContent();
    }

    public override void OnUpdate()
    {
        SetAppBackgroundMode();
        if (Client != null && Client.m_Instance != null)
        {
            Client.m_Instance.PollEvents();

            if (Client.m_IsReady) PlayersManager.UpdateLocalPlayer();
        }

        if (Server != null && Server.m_IsReady) Server.Update();
    }

    public static void ReimplementConsole()
    {
        if (uConsole.m_Instance == null)
        {
            Logger.Log("No uConsole present, creating one.");
            var ConsoleReference = Addressables.LoadAssetAsync<GameObject>("uConsole").WaitForCompletion();
            if (ConsoleReference != null)
            {
                var ConsoleObj = Object.Instantiate(ConsoleReference);
                if (ConsoleObj)
                    uConsole.m_Instance = ConsoleObj.GetComponent<uConsole>();
                else
                    Logger.Log(System.ConsoleColor.Red, "Can't assign uConsole!");
            }
            else
            {
                Logger.Log(System.ConsoleColor.Red, "Can't load uConsole!");
            }
        }
    }

    public static string GetCurrentSceneName()
    {
        if (GameManager.m_SceneTransitionData != null)
        {
            if (string.IsNullOrEmpty(GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent)) return "Empty";
            return GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent;
        }

        return "Empty";
    }

    public static bool IsGameplayScene(string Scene = "")
    {
        if (Scene == "") Scene = GetCurrentSceneName();
        if (Scene == "Empty" || Scene == "Boot" || Scene == "MainMenu") return false;
        return true;
    }

    public static void SetupSurvivalSettings(string GameMode, int Seed, string Region)
    {
        var EMM = GameManager.GetExperienceModeManagerComponent();
        GameModeConfig SelectedMode = null;
        RegionSpecification SelectedRegion = null;
        foreach (var Mode in EMM.m_AvailableGameModes)
            if (GameMode == Mode.name)
            {
                SelectedMode = Mode;
                break;
            }

        var Panel_Regions = InterfaceManager.GetPanel<Panel_SelectRegion_Map>();

        foreach (var R in Panel_Regions.m_Items)
            if (R.name == Region)
            {
                SelectedRegion = R.m_RegionSpec;
                break;
            }

        EMM.SetGameModeConfig(SelectedMode);
        GameManager.m_SceneTransitionData.m_GameRandomSeed = Seed;
        GameManager.m_StartRegion = SelectedRegion;
        GameManager.m_Instance.LaunchSandbox();
    }
}
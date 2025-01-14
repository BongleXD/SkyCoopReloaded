﻿using Il2Cpp;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SkyCoop;

public static class PlayersManager
{
    public static List<Comps.NetworkPlayer> s_Players = new();
    public static LocalPlayerData m_LocalPlayerData = new();

    public static Comps.NetworkPlayer CreatePlayer(int PlayerID)
    {
        var Reference = AssetManager.GetAssetFromBundle<GameObject>("SkyCoopPlayer");

        if (Reference)
        {
            var Player = Object.Instantiate(Reference);
            if (Player)
            {
                Object.DontDestroyOnLoad(Player); // if scence change, this object won't be destroyed.

                var NP = Player.AddComponent<Comps.NetworkPlayer>();
                NP.m_Animator = Player.GetComponent<Animator>();
                NP.LoadEquipment();

                NP.m_PlayerID = PlayerID;

                Player.SetActive(false);

                return NP;
            }
        }

        return null;
    }

    public static Comps.NetworkPlayer GetPlayer(int Index)
    {
        return s_Players[Index];
    }

    public static void InitilizePlayers(int PlayersCount)
    {
        m_LocalPlayerData = new LocalPlayerData();

        // Trying to re-use such complex objects as much as possible.
        // So in for some reason we have less or more characters already exist
        // (for example join server with less/more Max Players), we just add/remove 
        // Instead of re-creating whole list of objects.
        while (s_Players.Count != PlayersCount)
            if (s_Players.Count > PlayersCount)
            {
                var Index = s_Players.Count - 1;
                var Player = s_Players[Index];
                if (Player != null) Object.Destroy(Player.gameObject);
                s_Players.RemoveAt(Index);
            }
            else if (s_Players.Count < PlayersCount)
            {
                var Index = s_Players.Count;
                var Player = CreatePlayer(Index);
                if (Player == null)
                {
                    Logger.Log(ConsoleColor.Red,
                        "[PlayersManager][InitilizePlayers] Wasn't able to create player object!");
                    break; // Else, we going to go to infinite loop.
                }

                s_Players.Add(Player);
            }
    }

    public static Comps.NetworkPlayer.Actions GetCurrentAction()
    {
        var bk = InterfaceManager.GetPanel<Panel_BreakDown>();
        if (bk && bk.IsBreakingDown()) return Comps.NetworkPlayer.Actions.Harvesting;
        var Diagnos = InterfaceManager.GetPanel<Panel_Diagnosis>();
        if (Diagnos && Diagnos.TreatmentInProgress()) return Comps.NetworkPlayer.Actions.Harvesting;
        var bh = InterfaceManager.GetPanel<Panel_BodyHarvest>();
        if (bh && bh.m_BodyHarvest) return Comps.NetworkPlayer.Actions.Harvesting;
        var PM = GameManager.GetPlayerManagerComponent();
        if (PM.ActiveInteraction != null &&
            PM.ActiveInteraction.GetInteractiveObject().GetComponent<HarvestableInteraction>())
            return Comps.NetworkPlayer.Actions.Harvesting;

        var CMode = PM.GetControlMode();
        if (CMode == PlayerControlMode.AimRevolver) return Comps.NetworkPlayer.Actions.PistolAim;
        if (CMode == PlayerControlMode.StartingFire) return Comps.NetworkPlayer.Actions.Igniting;
        if (CMode == PlayerControlMode.DeployRope || CMode == PlayerControlMode.TakeRope)
            return Comps.NetworkPlayer.Actions.Harvesting;
        if (PM.m_ItemInHands)
        {
            if (PM.m_ItemInHands.name == "GEAR_Rifle")
            {
                if (PM.m_ItemInHands.m_GunItem.IsAiming()) return Comps.NetworkPlayer.Actions.RifleAim;
            }
            else if (PM.m_ItemInHands.name == "GEAR_FlareGun")
            {
                if (PM.m_ItemInHands.m_GunItem.IsAiming()) return Comps.NetworkPlayer.Actions.PistolAim;
            }
        }

        return Comps.NetworkPlayer.Actions.None;
    }

    public static void UpdateLocalPlayer()
    {
        if (!GameManager.s_IsGameplaySuspended)
        {
            var Scene = ModMain.GetCurrentSceneName();
            if (ModMain.IsGameplayScene(Scene))
                if (GameManager.m_PlayerObject)
                {
                    var T = GameManager.GetPlayerTransform();
                    if (m_LocalPlayerData.m_LastSentPosition != T.position)
                    {
                        m_LocalPlayerData.m_LastSentPosition = T.position;
                        ClientSend.SendPosition(T.position);
                    }

                    if (m_LocalPlayerData.m_LastSentRotation != T.rotation)
                    {
                        m_LocalPlayerData.m_LastSentRotation = T.rotation;
                        ClientSend.SendRotation(T.rotation);
                    }

                    var PM = GameManager.GetPlayerManagerComponent();
                    if (PM)
                    {
                        var Gi = PM.m_ItemInHands;
                        var HoldingGear = Gi ? Gi.name : "";
                        if (m_LocalPlayerData.m_GearName != HoldingGear)
                        {
                            m_LocalPlayerData.m_GearName = HoldingGear;
                            ClientSend.SendHoldingGear(HoldingGear, m_LocalPlayerData.m_GearVariant);
                        }

                        if (m_LocalPlayerData.m_LastSentCrouch != PM.PlayerIsCrouched())
                        {
                            m_LocalPlayerData.m_LastSentCrouch = PM.PlayerIsCrouched();
                            ClientSend.SendCrouch(m_LocalPlayerData.m_LastSentCrouch);
                        }

                        var Action = GetCurrentAction();
                        if (m_LocalPlayerData.m_LastSentAction != Action)
                        {
                            m_LocalPlayerData.m_LastSentAction = Action;
                            ClientSend.SendAction((int)Action);
                        }
                    }
                }

            if (m_LocalPlayerData.m_LastSentScene != Scene)
            {
                m_LocalPlayerData.m_LastSentScene = Scene;
                ClientSend.SendScene(Scene);
            }
        }
    }

    public class LocalPlayerData
    {
        public string m_GearName = "";
        public int m_GearVariant = 0;
        public Comps.NetworkPlayer.Actions m_LastSentAction = Comps.NetworkPlayer.Actions.None;

        public bool m_LastSentCrouch;
        public Vector3 m_LastSentPosition = Vector3.zero;
        public Quaternion m_LastSentRotation = Quaternion.identity;
        public string m_LastSentScene = "MainMenu";
    }
}
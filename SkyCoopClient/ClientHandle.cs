﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2Cpp;
using LiteNetLib;
using LiteNetLib.Utils;
using MelonLoader;
using SkyCoopServer;
using UnityEngine;

namespace SkyCoop
{
    public class ClientHandle
    {
        public static void Welcome(NetDataReader Reader)
        {
            string Message = Reader.ReadString();
            Logger.Log(ConsoleColor.Cyan,"Server welcomes me with message: "+ Message);
            ClientSend.Welcome();
            MenuHook.RemovePleaseWait();
            MenuHook.DoPleaseWait("Please wait...", "Getting data about server...");
            //MenuHook.DoOKMessage("Connected!", Message);
        }

        public static void ServerConfig(NetDataReader Reader)
        {
            int PlayersMax = Reader.GetInt();
            int Seed = Reader.GetInt();
            string StartingRegion = Reader.ReadString();
            string GameMode = Reader.ReadString();

            PlayersManager.InitilizePlayers(PlayersMax);

            Logger.Log(ConsoleColor.Cyan, "Server config");
            Logger.Log(ConsoleColor.Cyan, "PlayersMax: " + PlayersMax);
            Logger.Log(ConsoleColor.Cyan, "Seed: " + Seed);
            Logger.Log(ConsoleColor.Cyan, "StartingRegion: "+ StartingRegion);
            Logger.Log(ConsoleColor.Cyan, "GameMode: " + GameMode);

            ModMain.Client.m_IsReady = true;
            MenuHook.RemovePleaseWait();
            ModMain.SetupSurvivalSettings(GameMode, Seed, StartingRegion);
        }

        public static void ClientPosition(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            Vector3 Position = Reader.ReadVector3Unity();

            Comps.NetworkPlayer Player = PlayersManager.GetPlayer(PlayerID);
            if(Player)
            {
                Player.SetPosition(Position);
            }
        }

        public static void ClientRotation(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            Quaternion Rotation = Reader.ReadQuaternionUnity();

            Comps.NetworkPlayer Player = PlayersManager.GetPlayer(PlayerID);
            if (Player)
            {
                Player.SetRotation(Rotation);
            }
        }

        public static void ClientSceneNotification(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            bool Present = Reader.GetBool();

            Comps.NetworkPlayer Player = PlayersManager.GetPlayer(PlayerID);
            if (Player)
            {
                bool PreviousState = Player.gameObject.activeSelf;
                
                if (Present)
                {
                    Player.KeepVisible();
                } else
                {
                    Player.gameObject.SetActive(false);
                }

                if(Present != PreviousState)
                {
                    Logger.Log("(ClientSceneNotification) Player ID " + PlayerID + " Visible " + Present);
                }
            }
        }
        public static void ClientHoldingGear(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            string GearName = Reader.ReadString();
            int GearVariant = Reader.GetInt();
            Comps.NetworkPlayer Player = PlayersManager.GetPlayer(PlayerID);
            if (Player)
            {
                Player.SetGear(GearName, GearVariant);
            }
        }

        public static void ClientCrouch(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            bool IsCrouching = Reader.GetBool();
            Comps.NetworkPlayer Player = PlayersManager.GetPlayer(PlayerID);
            if (Player)
            {
                Player.SetCrouching(IsCrouching);
            }
        }
        public static void ClientAction(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            int Action = Reader.GetInt();
            Comps.NetworkPlayer Player = PlayersManager.GetPlayer(PlayerID);
            if (Player)
            {
                Player.SetAcation(Action);
            }
        }
        public static void ClientFire(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            Comps.NetworkPlayer Player = PlayersManager.GetPlayer(PlayerID);
            if (Player)
            {
                Player.DoFire();
            }
        }
    }
}

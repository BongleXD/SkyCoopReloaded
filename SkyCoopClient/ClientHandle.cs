using System;
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
            int MessageLength = Reader.GetInt();
            string Message = Reader.GetString(MessageLength);
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
            string StartingRegion = Reader.GetString(Reader.GetInt());
            string GameMode = Reader.GetString(Reader.GetInt());

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
            Vector3 Position = new Vector3(Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat());

            //Logger.Log("(ClientPosition) Player ID " + PlayerID + Position.ToString());

            Comps.NetworkPlayer Player = PlayersManager.GetPlayer(PlayerID);
            if(Player)
            {
                Player.SetPosition(Position);
            }
        }

        public static void ClientRotation(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            Quaternion Rotation = new Quaternion(Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat());

            //Logger.Log("(ClientRotation) Player ID " + PlayerID + Rotation.ToString());

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

            Logger.Log("(ClientSceneNotification) Player ID "+PlayerID+" Visible "+ Present);

            Comps.NetworkPlayer Player = PlayersManager.GetPlayer(PlayerID);
            if (Player)
            {
                if (Present)
                {
                    Player.KeepVisible();
                } else
                {
                    Player.gameObject.SetActive(false);
                }
            }
        }
    }
}

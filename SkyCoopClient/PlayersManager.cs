using Il2Cpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SkyCoop
{
    public static class PlayersManager
    {
        public static List<Comps.NetworkPlayer> s_Players = new List<Comps.NetworkPlayer>();
        public static LocalPlayerData m_LocalPlayerData = new LocalPlayerData();
        public class LocalPlayerData
        {
            public Vector3 m_LastSentPosition = Vector3.zero;
            public Quaternion m_LastSentRotation = Quaternion.identity;
            public string m_LastSentScene = "MainMenu";
        }

        public static Comps.NetworkPlayer CreatePlayer(int PlayerID)
        {
            GameObject Reference = AssetManager.GetAssetFromBundle<GameObject>("multiplayerPlayer");

            if(Reference)
            {
                GameObject Player = GameObject.Instantiate(Reference);
                if(Player)
                {
                    UnityEngine.Object.DontDestroyOnLoad(Player); // if scence change, this object won't be destroyed.

                    Comps.NetworkPlayer NP = Player.AddComponent<Comps.NetworkPlayer>();

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
            {
                if (s_Players.Count > PlayersCount)
                {
                    int Index = s_Players.Count - 1;
                    Comps.NetworkPlayer Player = s_Players[Index];
                    if (Player != null)
                    {
                        UnityEngine.Object.Destroy(Player.gameObject);
                    }
                    s_Players.RemoveAt(Index);
                } else if (s_Players.Count < PlayersCount)
                {
                    int Index = s_Players.Count;
                    Comps.NetworkPlayer Player = CreatePlayer(Index);
                    if (Player == null)
                    {
                        Logger.Log(ConsoleColor.Red, "[PlayersManager][InitilizePlayers] Wasn't able to create player object!");
                        break; // Else, we going to go to infinite loop.
                    }
                    s_Players.Add(Player);
                }
            }
        }

        public static void UpdateLocalPlayer()
        {
            if(!GameManager.s_IsGameplaySuspended)
            {
                string Scene = ModMain.GetCurrentSceneName();
                if (ModMain.IsGameplayScene(Scene))
                {
                    if (GameManager.m_PlayerObject)
                    {
                        Transform T = GameManager.GetPlayerTransform();
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
                    }
                }

                if (m_LocalPlayerData.m_LastSentScene != Scene)
                {
                    m_LocalPlayerData.m_LastSentScene = Scene;
                    ClientSend.SendScene(Scene);
                }
            }
        }
    }
}

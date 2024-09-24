using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SkyCoopServer
{
    public class PlayersDataManager
    {
        public List<DataStr.PlayerData> m_Players = new List<DataStr.PlayerData>();

        private Server s_Server;

        public PlayersDataManager(Server ServerInstance) 
        {
            s_Server = ServerInstance;
        }

        public void InitilizePlayers(int MaxPlayers)
        {
            m_Players.Clear(); // Clear instead of creating new.
            for (int i = 0; i < MaxPlayers; i++)
            {
                m_Players.Add(new DataStr.PlayerData(i));
            }
        }

        public DataStr.PlayerData GetPlayer(int Index)
        {
            return m_Players[Index];
        }

        public List<DataStr.PlayerData> GetPlayersOnScene(string Scene)
        {
            List<DataStr.PlayerData> ScenePlayers = new List<DataStr.PlayerData>();
            if (Scene == "Empty" || Scene == "" || Scene == "Boot" || Scene == "MainMenu")
            {
                return ScenePlayers;
            }
            foreach (DataStr.PlayerData Player in m_Players)
            {
                if(Player.m_Scene == Scene)
                {
                    ScenePlayers.Add(Player);
                }
            }
            return ScenePlayers;
        }

        public void PlayerMoved(int Index, Vector3 Position, bool Broadcast = true)
        {
            DataStr.PlayerData Player = GetPlayer(Index);
            if(Player != null )
            {
                Player.m_Position = Position;

                if(Broadcast)
                {
                    if (s_Server != null)
                    {
                        List<DataStr.PlayerData> Players = GetPlayersOnScene(Player.m_Scene);

                        foreach (DataStr.PlayerData OnScenePlayer in Players)
                        {
                            if(OnScenePlayer.m_PlayerID != Player.m_PlayerID)
                            {
                                ServerSend.SendPosition(s_Server.GetClient(OnScenePlayer.m_PlayerID), Position, Player.m_PlayerID);
                            }
                        }
                    }
                }
            }
        }

        public void PlayerRotated(int Index, Quaternion Rotation, bool Broadcast = true)
        {
            DataStr.PlayerData Player = GetPlayer(Index);
            if (Player != null)
            {
                Player.m_Rotation = Rotation;

                if (Broadcast)
                {
                    if (s_Server != null)
                    {
                        List<DataStr.PlayerData> Players = GetPlayersOnScene(Player.m_Scene);

                        foreach (DataStr.PlayerData OnScenePlayer in Players)
                        {
                            if (OnScenePlayer.m_PlayerID != Player.m_PlayerID)
                            {
                                ServerSend.SendRotation(s_Server.GetClient(OnScenePlayer.m_PlayerID), Rotation, Player.m_PlayerID);
                            }
                        }
                    }
                }
            }
        }
        public void PlayerChangeScene(int Index, string Scene, bool Broadcast = true)
        {
            DataStr.PlayerData Player = GetPlayer(Index);
            if (Player != null)
            {
                Player.m_Scene = Scene;

                if (Broadcast)
                {
                    if (s_Server != null)
                    {
                        
                        // Notify all other clients if this player enters/leaves scene that client is on aswell.
                        // For example Client1 and Client2 on Scene A, and Client3 on scene B, and Client4 on scene C.
                        // When Client1 going to scene B.
                        // Client2 will recive client1 Left Scene message.
                        // Client3 will recive client1 Enter Scene message.
                        // Client4 will recive NOTHING.
                        foreach (DataStr.PlayerData OtherPlayer in m_Players)
                        {
                            if(OtherPlayer.m_Scene != "")
                            {
                                if (OtherPlayer.m_PlayerID != Player.m_PlayerID)
                                {
                                    ServerSend.SendPlayerSceneNotification(
                                        s_Server.GetClient(OtherPlayer.m_PlayerID),
                                        OtherPlayer.m_Scene == Player.m_Scene,
                                        Player.m_PlayerID);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

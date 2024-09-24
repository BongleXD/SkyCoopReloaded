using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SkyCoopServer
{
    public class DataStr
    {
        public class ServerConfig
        {
            public int m_MaxPlayers = 4;
            public string m_StartingRegion = "ForlornMuskeg";
            public int m_Seed = 777777;
            public string m_GameMode = "Stalker";
        }
        public class PlayerData
        {
            public int m_PlayerID = 0;
            
            public Vector3 m_Position = new Vector3(0, 0, 0);
            public Quaternion m_Rotation = new Quaternion(0, 0, 0, 0);

            public string m_Scene = "";

            public PlayerData(int PlayerID)
            {
                m_PlayerID = PlayerID;
            }
        }
    }
}

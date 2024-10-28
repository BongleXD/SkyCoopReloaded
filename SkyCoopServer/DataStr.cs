using System.Numerics;

namespace SkyCoopServer;

public class DataStr
{
    public class ServerConfig
    {
        public string m_GameMode = "Stalker";
        public int m_MaxPlayers = 4;
        public int m_Seed = 777777;
        public string m_StartingRegion = "MysteryLake";
    }

    public class PlayerData
    {
        public int m_PlayerID;

        public Vector3 m_Position = new(0, 0, 0);
        public Quaternion m_Rotation = new(0, 0, 0, 0);

        public string m_Scene = "";
        public PlayerVisualData m_VisualData = new();

        public PlayerData(int PlayerID)
        {
            m_PlayerID = PlayerID;
        }
    }

    public class PlayerVisualData
    {
        public bool m_Crouch = false;
        public string m_GearInHands = "";
        public int m_GearVariant = 0;
        public int m_LatAction = 0;
    }
}
using System.Numerics;

namespace SkyCoopServer;

public class PlayersDataManager
{
    public List<DataStr.PlayerData> m_Players = new();

    public bool m_RecursiveDebug = true;

    private readonly Server s_Server;

    public PlayersDataManager(Server ServerInstance)
    {
        s_Server = ServerInstance;
    }

    public void InitilizePlayers(int MaxPlayers)
    {
        m_Players.Clear(); // Clear instead of creating new.
        for (var i = 0; i < MaxPlayers; i++) m_Players.Add(new DataStr.PlayerData(i));
    }

    public DataStr.PlayerData GetPlayer(int Index)
    {
        return m_Players[Index];
    }

    public List<DataStr.PlayerData> GetPlayersOnScene(string Scene)
    {
        var ScenePlayers = new List<DataStr.PlayerData>();
        if (Scene == "Empty" || Scene == "" || Scene == "Boot" || Scene == "MainMenu") return ScenePlayers;
        foreach (var Player in m_Players)
            if (Player.m_Scene == Scene)
                ScenePlayers.Add(Player);
        return ScenePlayers;
    }

    public void PlayerMoved(int Index, Vector3 Position, bool Broadcast = true)
    {
        var Player = GetPlayer(Index);
        if (Player != null)
        {
            Player.m_Position = Position;

            if (Broadcast)
                if (s_Server != null)
                {
                    var Players = GetPlayersOnScene(Player.m_Scene);

                    foreach (var OnScenePlayer in Players)
                        if (OnScenePlayer.m_PlayerID != Player.m_PlayerID || m_RecursiveDebug)
                            ServerSend.SendPosition(s_Server.GetClient(OnScenePlayer.m_PlayerID), Position,
                                Player.m_PlayerID);
                }
        }
    }

    public void PlayerRotated(int Index, Quaternion Rotation, bool Broadcast = true)
    {
        var Player = GetPlayer(Index);
        if (Player != null)
        {
            Player.m_Rotation = Rotation;

            if (Broadcast)
                if (s_Server != null)
                {
                    var Players = GetPlayersOnScene(Player.m_Scene);

                    foreach (var OnScenePlayer in Players)
                        if (OnScenePlayer.m_PlayerID != Player.m_PlayerID || m_RecursiveDebug)
                            ServerSend.SendRotation(s_Server.GetClient(OnScenePlayer.m_PlayerID), Rotation,
                                Player.m_PlayerID);
                }
        }
    }

    public void SceneAlign()
    {
        Console.WriteLine("SceneAlign");
        var PlayerIndexes = s_Server.GetClientsIndexs();

        foreach (var PlayerID in PlayerIndexes)
        {
            var Player = GetPlayer(PlayerID);
            foreach (var OtherPlayerID in PlayerIndexes)
                if (OtherPlayerID != PlayerID || m_RecursiveDebug)
                {
                    var OtherPlayer = GetPlayer(OtherPlayerID);

                    var OtherPlayerClient = s_Server.GetClient(OtherPlayerID);
                    var PlayerClient = s_Server.GetClient(PlayerID);

                    ServerSend.SendPlayerSceneNotification(OtherPlayerClient, OtherPlayer.m_Scene == Player.m_Scene,
                        PlayerID);
                    ServerSend.SendPlayerSceneNotification(PlayerClient, OtherPlayer.m_Scene == Player.m_Scene,
                        OtherPlayerID);
                }
        }
    }

    public void PlayerChangeGear(int Index, string GearName, int GearVariant, bool Broadcast = true)
    {
        var Player = GetPlayer(Index);
        if (Player != null)
        {
            Player.m_VisualData.m_GearInHands = GearName;
            Player.m_VisualData.m_GearVariant = GearVariant;

            if (Broadcast)
                if (s_Server != null)
                    foreach (var OtherPlayerID in s_Server.GetClientsIndexs())
                        if (OtherPlayerID != Index || m_RecursiveDebug)
                        {
                            var OtherPlayer = GetPlayer(OtherPlayerID);

                            if (OtherPlayer.m_Scene == Player.m_Scene)
                                ServerSend.SendPlayerChangeGear(s_Server.GetClient(OtherPlayerID), GearName,
                                    GearVariant, Index);
                        }
        }
    }

    public void PlayerChangeScene(int Index, string Scene, bool Broadcast = true)
    {
        var Player = GetPlayer(Index);
        if (Player != null)
        {
            Player.m_Scene = Scene;

            if (Broadcast)
                if (s_Server != null)
                    foreach (var OtherPlayerID in s_Server.GetClientsIndexs())
                        if (OtherPlayerID != Index || m_RecursiveDebug)
                        {
                            var OtherPlayer = GetPlayer(OtherPlayerID);
                            ServerSend.SendPlayerSceneNotification(s_Server.GetClient(OtherPlayerID),
                                OtherPlayer.m_Scene == Player.m_Scene, Index);
                        }
        }
    }

    public void PlayerChangeCrouch(int Index, bool CrouchState, bool Broadcast = true)
    {
        var Player = GetPlayer(Index);
        if (Player != null)
        {
            Player.m_VisualData.m_Crouch = CrouchState;

            if (Broadcast)
                if (s_Server != null)
                    foreach (var OtherPlayerID in s_Server.GetClientsIndexs())
                        if (OtherPlayerID != Index || m_RecursiveDebug)
                        {
                            var OtherPlayer = GetPlayer(OtherPlayerID);

                            if (OtherPlayer.m_Scene == Player.m_Scene)
                                ServerSend.SendPlayerCrouch(s_Server.GetClient(OtherPlayerID), CrouchState, Index);
                        }
        }
    }

    public void PlayerChangeAction(int Index, int Action, bool Broadcast = true)
    {
        var Player = GetPlayer(Index);
        if (Player != null)
        {
            Player.m_VisualData.m_LatAction = Action;

            if (Broadcast)
                if (s_Server != null)
                    foreach (var OtherPlayerID in s_Server.GetClientsIndexs())
                        if (OtherPlayerID != Index || m_RecursiveDebug)
                        {
                            var OtherPlayer = GetPlayer(OtherPlayerID);

                            if (OtherPlayer.m_Scene == Player.m_Scene)
                                ServerSend.SendPlayerAction(s_Server.GetClient(OtherPlayerID), Action, Index);
                        }
        }
    }

    public void PlayerFire(int Index, bool Broadcast = true)
    {
        var Player = GetPlayer(Index);
        if (Player != null)
            if (Broadcast)
                if (s_Server != null)
                    foreach (var OtherPlayerID in s_Server.GetClientsIndexs())
                        if (OtherPlayerID != Index || m_RecursiveDebug)
                        {
                            var OtherPlayer = GetPlayer(OtherPlayerID);

                            if (OtherPlayer.m_Scene == Player.m_Scene)
                                ServerSend.SendPlayerFire(s_Server.GetClient(OtherPlayerID), Index);
                        }
    }
}
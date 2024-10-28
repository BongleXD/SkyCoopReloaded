using LiteNetLib.Utils;
using SkyCoopServer;

namespace SkyCoop;

public class ClientHandle
{
    public static void Welcome(NetDataReader Reader)
    {
        var Message = Reader.ReadString();
        Logger.Log(ConsoleColor.Cyan, "Server welcomes me with message: " + Message);
        ClientSend.Welcome();
        MenuHook.RemovePleaseWait();
        MenuHook.DoPleaseWait("Please wait...", "Getting data about server...");
        //MenuHook.DoOKMessage("Connected!", Message);
    }

    public static void ServerConfig(NetDataReader Reader)
    {
        var PlayersMax = Reader.GetInt();
        var Seed = Reader.GetInt();
        var StartingRegion = Reader.ReadString();
        var GameMode = Reader.ReadString();

        PlayersManager.InitilizePlayers(PlayersMax);

        Logger.Log(ConsoleColor.Cyan, "Server config");
        Logger.Log(ConsoleColor.Cyan, "PlayersMax: " + PlayersMax);
        Logger.Log(ConsoleColor.Cyan, "Seed: " + Seed);
        Logger.Log(ConsoleColor.Cyan, "StartingRegion: " + StartingRegion);
        Logger.Log(ConsoleColor.Cyan, "GameMode: " + GameMode);

        ModMain.Client.m_IsReady = true;
        MenuHook.RemovePleaseWait();
        ModMain.SetupSurvivalSettings(GameMode, Seed, StartingRegion);
    }

    public static void ClientPosition(NetDataReader Reader)
    {
        var PlayerID = Reader.GetInt();
        var Position = Reader.ReadVector3Unity();

        var Player = PlayersManager.GetPlayer(PlayerID);
        if (Player) Player.SetPosition(Position);
    }

    public static void ClientRotation(NetDataReader Reader)
    {
        var PlayerID = Reader.GetInt();
        var Rotation = Reader.ReadQuaternionUnity();

        var Player = PlayersManager.GetPlayer(PlayerID);
        if (Player) Player.SetRotation(Rotation);
    }

    public static void ClientSceneNotification(NetDataReader Reader)
    {
        var PlayerID = Reader.GetInt();
        var Present = Reader.GetBool();

        var Player = PlayersManager.GetPlayer(PlayerID);
        if (Player)
        {
            var PreviousState = Player.gameObject.activeSelf;

            if (Present)
                Player.KeepVisible();
            else
                Player.gameObject.SetActive(false);

            if (Present != PreviousState)
                Logger.Log("(ClientSceneNotification) Player ID " + PlayerID + " Visible " + Present);
        }
    }

    public static void ClientHoldingGear(NetDataReader Reader)
    {
        var PlayerID = Reader.GetInt();
        var GearName = Reader.ReadString();
        var GearVariant = Reader.GetInt();
        var Player = PlayersManager.GetPlayer(PlayerID);
        if (Player) Player.SetGear(GearName, GearVariant);
    }

    public static void ClientCrouch(NetDataReader Reader)
    {
        var PlayerID = Reader.GetInt();
        var IsCrouching = Reader.GetBool();
        var Player = PlayersManager.GetPlayer(PlayerID);
        if (Player) Player.SetCrouching(IsCrouching);
    }

    public static void ClientAction(NetDataReader Reader)
    {
        var PlayerID = Reader.GetInt();
        var Action = Reader.GetInt();
        var Player = PlayersManager.GetPlayer(PlayerID);
        if (Player) Player.SetAcation(Action);
    }

    public static void ClientFire(NetDataReader Reader)
    {
        var PlayerID = Reader.GetInt();
        var Player = PlayersManager.GetPlayer(PlayerID);
        if (Player) Player.DoFire();
    }
}
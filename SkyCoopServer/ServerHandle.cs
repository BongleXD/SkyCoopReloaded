using LiteNetLib;
using LiteNetLib.Utils;

namespace SkyCoopServer;

public class ServerHandle
{
    public static void Welcome(NetPeer Client, NetDataReader Reader, Server ServerInstance)
    {
        var Message = Reader.ReadString();
        Console.WriteLine("Сlient " + Client.Id + " responced with message: " + Message);
        ServerSend.ServerConfig(Client, ServerInstance.m_Config);
    }

    public static void ClientPosition(NetPeer Client, NetDataReader Reader, Server ServerInstance)
    {
        var Position = Reader.ReadVector3();
        ServerInstance.m_PlayersData.PlayerMoved(Client.Id, Position);
    }

    public static void ClientRotation(NetPeer Client, NetDataReader Reader, Server ServerInstance)
    {
        var Rotation = Reader.ReadQuaternion();
        ServerInstance.m_PlayersData.PlayerRotated(Client.Id, Rotation);
    }

    public static void ClientScene(NetPeer Client, NetDataReader Reader, Server ServerInstance)
    {
        var Scene = Reader.ReadString();
        Console.WriteLine("(ClientScene) Client " + Client.Id + " sent Scene " + Scene);
        ServerInstance.m_PlayersData.PlayerChangeScene(Client.Id, Scene);
    }

    public static void ClientHoldingGear(NetPeer Client, NetDataReader Reader, Server ServerInstance)
    {
        var GearName = Reader.ReadString();
        var GearVariant = Reader.GetInt();
        ServerInstance.m_PlayersData.PlayerChangeGear(Client.Id, GearName, GearVariant);
    }

    public static void ClientCrouch(NetPeer Client, NetDataReader Reader, Server ServerInstance)
    {
        var IsCrouch = Reader.GetBool();
        ServerInstance.m_PlayersData.PlayerChangeCrouch(Client.Id, IsCrouch);
    }

    public static void ClientAction(NetPeer Client, NetDataReader Reader, Server ServerInstance)
    {
        var Action = Reader.GetInt();
        ServerInstance.m_PlayersData.PlayerChangeAction(Client.Id, Action);
    }

    public static void ClientFire(NetPeer Client, NetDataReader Reader, Server ServerInstance)
    {
        ServerInstance.m_PlayersData.PlayerFire(Client.Id);
    }
}
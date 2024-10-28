using LiteNetLib.Utils;
using SkyCoopServer;
using UnityEngine;

namespace SkyCoop;

public class ClientSend
{
    public static void SendToHost(NetDataWriter writer)
    {
        if (ModMain.Client != null && ModMain.Client.m_Instance != null) ModMain.Client.SendToHost(writer);
    }

    public static void Welcome()
    {
        //TODO: Send here MAC address and nick name.

        var Message = "I am connected!";
        var writer = new NetDataWriter();
        writer.Put((int)Packet.Type.Welcome);
        writer.Write(Message);
        SendToHost(writer);
    }

    public static void SendPosition(Vector3 Position)
    {
        var writer = new NetDataWriter();
        writer.Put((int)Packet.Type.ClientPosition);
        writer.Write(Position);
        SendToHost(writer);
    }

    public static void SendRotation(Quaternion Rotation)
    {
        var writer = new NetDataWriter();

        writer.Put((int)Packet.Type.ClientRotation);
        writer.Write(Rotation);
        SendToHost(writer);
    }

    public static void SendScene(string Scene)
    {
        var writer = new NetDataWriter();

        writer.Put((int)Packet.Type.ClientScene);
        writer.Write(Scene);

        SendToHost(writer);
    }

    public static void SendHoldingGear(string GearName, int GearVariant)
    {
        var writer = new NetDataWriter();

        writer.Put((int)Packet.Type.ClientHoldigGear);
        writer.Write(GearName);
        writer.Put(GearVariant);
        SendToHost(writer);
    }

    public static void SendCrouch(bool Crouch)
    {
        var writer = new NetDataWriter();
        writer.Put((int)Packet.Type.ClientCrouch);
        writer.Put(Crouch);
        SendToHost(writer);
    }

    public static void SendAction(int Action)
    {
        var writer = new NetDataWriter();
        writer.Put((int)Packet.Type.ClientAction);
        writer.Put(Action);
        SendToHost(writer);
    }

    public static void SendFire()
    {
        var writer = new NetDataWriter();
        writer.Put((int)Packet.Type.ClientFire);
        SendToHost(writer);
    }
}
using System.Numerics;
using LiteNetLib.Utils;

namespace SkyCoopServer;

public static class Packet
{
    public enum Type
    {
        Welcome = 0,
        CFG,
        ClientPosition,
        ClientRotation,
        ClientScene,
        ClientHoldigGear,
        ClientCrouch,
        ClientAction,
        ClientFire
    }

    public static void Write(this NetDataWriter Writer, string Message)
    {
        Writer.Put(Message.Length);
        Writer.Put(Message);
    }

    public static string ReadString(this NetDataReader Reader)
    {
        return Reader.GetString(Reader.GetInt());
    }

    public static void Write(this NetDataWriter Writer, Vector3 v3)
    {
        Writer.Put(v3.X);
        Writer.Put(v3.Y);
        Writer.Put(v3.Z);
    }

    public static Vector3 ReadVector3(this NetDataReader Reader)
    {
        var v3 = new Vector3(Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat());
        return v3;
    }

    public static void Write(this NetDataWriter Writer, Quaternion quat)
    {
        Writer.Put(quat.X);
        Writer.Put(quat.Y);
        Writer.Put(quat.Z);
        Writer.Put(quat.W);
    }

    public static Quaternion ReadQuaternion(this NetDataReader Reader)
    {
        var quat = new Quaternion(Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat());
        return quat;
    }
}
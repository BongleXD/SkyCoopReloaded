using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SkyCoopServer
{
    public class ServerSend
    {
        public static void Welcome(NetPeer Client, string Message)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.Welcome);
            writer.Put(Message.Length);
            writer.Put(Message);
            Client.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public static void ServerConfig(NetPeer Client, DataStr.ServerConfig CFG)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.CFG);
            writer.Put(CFG.m_MaxPlayers);
            writer.Put(CFG.m_Seed);
            writer.Put(CFG.m_StartingRegion.Length);
            writer.Put(CFG.m_StartingRegion);
            writer.Put(CFG.m_GameMode.Length);
            writer.Put(CFG.m_GameMode);
            Client.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendPosition(NetPeer Client, Vector3 Position, int FromClient)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientPosition);
            writer.Put(FromClient);
            writer.Put(Position.X);
            writer.Put(Position.Y);
            writer.Put(Position.Z);
            Client.Send(writer, DeliveryMethod.Unreliable);
        }

        public static void SendRotation(NetPeer Client, Quaternion Rotation, int FromClient)
        {
            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.ClientRotation);
            writer.Put(FromClient);
            writer.Put(Rotation.X);
            writer.Put(Rotation.Y);
            writer.Put(Rotation.Z);
            writer.Put(Rotation.W);
            Client.Send(writer, DeliveryMethod.Unreliable);
        }

        public static void SendPlayerSceneNotification(NetPeer Client, bool Present, int FromClient)
        {
            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.ClientScene);
            writer.Put(FromClient);
            writer.Put(Present);
            Client.Send(writer, DeliveryMethod.ReliableOrdered);
        }
    }
}

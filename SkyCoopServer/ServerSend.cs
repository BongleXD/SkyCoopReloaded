using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCoopServer
{
    public class ServerSend
    {
        public static void Welcome(NetPeer Client, string Message)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Server.PacketType.Welcome);
            writer.Put(Message.Length);
            writer.Put(Message);
            Client.Send(writer, DeliveryMethod.ReliableOrdered);
        }
    }
}

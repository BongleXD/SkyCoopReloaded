using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyCoopServer;

namespace SkyCoop
{
    public class ClientSend
    {
        public static void SendToHost(NetDataWriter writer)
        {
            if (ModMain.Client != null && ModMain.Client.m_Instance != null)
            {
                ModMain.Client.SendToHost(writer);
            }
        }

        public static void Welcome()
        {
            //TODO: Send here MAC address and nick name.

            string Message = "I am connected!";
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.Welcome);
            writer.Put(Message.Length);
            writer.Put(Message);
            SendToHost(writer);
        }
    }
}

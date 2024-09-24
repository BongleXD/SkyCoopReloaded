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
    public class ServerHandle
    {
        public static void Welcome(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            int MessageLength = Reader.GetInt();
            string Message = Reader.GetString(MessageLength);
            Console.WriteLine("Сlient "+ Client .Id+ " responced with message: " + Message);
            ServerSend.ServerConfig(Client, ServerInstance.m_Config);
        }

        public static void ClientPosition(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            Vector3 Position = new Vector3(Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat());

            //Console.WriteLine("(ClientPosition) Client " + Client.Id + " position " + Position.ToString());

            ServerInstance.m_PlayersData.PlayerMoved(Client.Id, Position);
        }

        public static void ClientRotation(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            Quaternion Rotation = new Quaternion(Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat());
            //Console.WriteLine("(ClientRotation) Client " + Client.Id + " rotation "+ Rotation.ToString());
            ServerInstance.m_PlayersData.PlayerRotated(Client.Id, Rotation);
        }

        public static void ClientScene(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string Scene = Reader.GetString(Reader.GetInt());
            Console.WriteLine("(ClientScene) Client " + Client.Id + " sent Scene "+Scene);
            ServerInstance.m_PlayersData.PlayerChangeScene(Client.Id, Scene);

        }
    }
}

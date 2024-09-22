using LiteNetLib;
using LiteNetLib.Utils;

namespace SkyCoopServer
{
    public class Server
    {
        public int m_Port = 26950;
        public int m_MaxPlayers = 4;

        public EventBasedNetListener m_Listener;
        public NetManager m_Instance;
        public bool m_IsReady = false;

        public delegate void PacketHandler(NetPeer Client, NetDataReader Reader);
        public static Dictionary<int, PacketHandler> s_packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)Packet.Type.Welcome, ServerHandle.Welcome },
        };

        public static void ExecutePacketEvent(int PacketID, NetPeer Client, NetDataReader Reader)
        {
            PacketHandler Handle;
            if (s_packetHandlers.TryGetValue(PacketID, out Handle))
            {
                Handle(Client, Reader);
            }
        }

        public Server()
        {
            m_Listener = new EventBasedNetListener();
            m_Instance = new NetManager(m_Listener);
        }

        public void StartServer()
        {
            StartServer(m_Port, m_MaxPlayers);
        }

        public void StartServer(int port, int maxPlayers, string key = "key")
        {
            Console.WriteLine("Starting server");
            m_Instance.Start(port);

            m_Listener.ConnectionRequestEvent += request =>
            {
                if (m_Instance.ConnectedPeersCount < maxPlayers)
                    request.AcceptIfKey(key);
                else
                    request.Reject();
            };

            m_Listener.PeerConnectedEvent += peer =>
            {
                Console.WriteLine("We got connection: {0}", peer+" assigned them as "+ peer.Id);
                ServerSend.Welcome(peer, "Welcome client "+peer.Id);
            };

            m_Listener.PeerDisconnectedEvent += (peer, message) =>
            {
                Console.WriteLine("Client", peer.Id + " disconnected " + message.Reason.ToString());
            };

            m_Listener.NetworkLatencyUpdateEvent += (peer, ping) =>
            {
                //Console.WriteLine("Ping to Client "+peer.Id+": " + ping);
            };
            m_Listener.NetworkReceiveEvent += (fromPeer, dataReader, channel, deliveryMethod) =>
            {
                int PacketID = dataReader.GetInt();

                ExecutePacketEvent(PacketID, fromPeer, dataReader);

                dataReader.Recycle();
            };

            m_IsReady = true;
            Console.WriteLine($"Server is started port={port}");
        }
    }
}

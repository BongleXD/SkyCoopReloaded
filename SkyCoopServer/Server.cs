using LiteNetLib;
using LiteNetLib.Utils;

namespace SkyCoopServer
{
    public class Server
    {
        public int m_Port = 26950;

        public DataStr.ServerConfig m_Config = new DataStr.ServerConfig();
        public EventBasedNetListener m_Listener;
        public NetManager m_Instance;
        public bool m_IsReady = false;

        // Data Sync Instances
        public PlayersDataManager m_PlayersData;


        public delegate void PacketHandler(NetPeer Client, NetDataReader Reader, Server ServerInstance);
        public static Dictionary<int, PacketHandler> s_packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)Packet.Type.Welcome, ServerHandle.Welcome },
            { (int)Packet.Type.ClientPosition, ServerHandle.ClientPosition },
            { (int)Packet.Type.ClientRotation, ServerHandle.ClientRotation },
            { (int)Packet.Type.ClientScene, ServerHandle.ClientScene },
        };

        public void ExecutePacketEvent(int PacketID, NetPeer Client, NetDataReader Reader)
        {
            PacketHandler Handle;
            if (s_packetHandlers.TryGetValue(PacketID, out Handle))
            {
                Handle(Client, Reader, this);
            }
        }

        public Server()
        {
            m_Listener = new EventBasedNetListener();
            m_Instance = new NetManager(m_Listener);

            //TODO: Loading Config
            m_Config = new DataStr.ServerConfig();

            // Data Sync Instances
            m_PlayersData = new PlayersDataManager(this);
        }

        public NetPeer GetClient(int Index)
        {
            if (m_Instance != null)
            {
                foreach (NetPeer Peer in m_Instance.ConnectedPeerList)
                {
                    if(Peer.Id == Index)
                    {
                        return Peer;
                    }
                }
            }
            return null;
        }

        public void Update()
        {
            if (m_Instance != null && m_IsReady)
            {
                m_Instance.PollEvents();
            }
        }

        public void StartServer()
        {
            StartServer(m_Port, m_Config.m_MaxPlayers);
        }

        public void StartServer(int port, int maxPlayers, string key = "key")
        {
            m_PlayersData.InitilizePlayers(maxPlayers);
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

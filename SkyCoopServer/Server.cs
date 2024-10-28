using LiteNetLib;
using LiteNetLib.Utils;

namespace SkyCoopServer;

public class Server
{
    public delegate void PacketHandler(NetPeer Client, NetDataReader Reader, Server ServerInstance);

    public static Dictionary<int, PacketHandler> s_packetHandlers = new()
    {
        { (int)Packet.Type.Welcome, ServerHandle.Welcome },
        { (int)Packet.Type.ClientPosition, ServerHandle.ClientPosition },
        { (int)Packet.Type.ClientRotation, ServerHandle.ClientRotation },
        { (int)Packet.Type.ClientScene, ServerHandle.ClientScene },
        { (int)Packet.Type.ClientHoldigGear, ServerHandle.ClientHoldingGear },
        { (int)Packet.Type.ClientCrouch, ServerHandle.ClientCrouch },
        { (int)Packet.Type.ClientAction, ServerHandle.ClientAction },
        { (int)Packet.Type.ClientFire, ServerHandle.ClientFire }
    };

    public DataStr.ServerConfig m_Config = new();
    public NetManager m_Instance;
    public bool m_IsReady;
    public EventBasedNetListener m_Listener;

    // Data Sync Instances
    public PlayersDataManager m_PlayersData;
    public int m_Port = 26950;

    public Server()
    {
        m_Listener = new EventBasedNetListener();
        m_Instance = new NetManager(m_Listener);

        //TODO: Loading Config
        m_Config = new DataStr.ServerConfig();

        // Data Sync Instances
        m_PlayersData = new PlayersDataManager(this);

        var timer1 = new Timer(EverySecond, null, 1000, 1000);
    }

    public void ExecutePacketEvent(int PacketID, NetPeer Client, NetDataReader Reader)
    {
        PacketHandler Handle;
        if (s_packetHandlers.TryGetValue(PacketID, out Handle)) Handle(Client, Reader, this);
    }

    public List<int> GetClientsIndexs()
    {
        var Indexes = new List<int>();
        if (m_Instance != null)
            foreach (var Peer in m_Instance.ConnectedPeerList)
                Indexes.Add(Peer.Id);
        return Indexes;
    }

    public NetPeer GetClient(int Index)
    {
        if (m_Instance != null)
            foreach (var Peer in m_Instance.ConnectedPeerList)
                if (Peer.Id == Index)
                    return Peer;
        return null;
    }

    public void Update()
    {
        if (m_Instance != null && m_IsReady) m_Instance.PollEvents();
    }

    public void EverySecond(object obj)
    {
        if (m_PlayersData != null) m_PlayersData.SceneAlign();
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
            Console.WriteLine("We got connection: {0}", peer + " assigned them as " + peer.Id);
            ServerSend.Welcome(peer, "Welcome client " + peer.Id);
        };

        m_Listener.PeerDisconnectedEvent += (peer, message) =>
        {
            Console.WriteLine("Client", peer.Id + " disconnected " + message.Reason);
        };

        m_Listener.NetworkLatencyUpdateEvent += (peer, ping) =>
        {
            //Console.WriteLine("Ping to Client "+peer.Id+": " + ping);
        };
        m_Listener.NetworkReceiveEvent += (fromPeer, dataReader, channel, deliveryMethod) =>
        {
            var PacketID = dataReader.GetInt();

            ExecutePacketEvent(PacketID, fromPeer, dataReader);

            dataReader.Recycle();
        };

        m_IsReady = true;
        Console.WriteLine($"Server is started port={port}");
    }
}
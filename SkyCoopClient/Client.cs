using LiteNetLib;
using LiteNetLib.Utils;
using MelonLoader;
using System.Net;

namespace SkyCoop
{
    public class Client
    {
        public static int m_ConnectPort = 26950;
        public static int m_LocalPort = 26951;
        public static int m_Protocol = 1;

        public static readonly int s_PacketTypesCount = Enum.GetValues(typeof(PacketType)).Length;
        public enum PacketType
        {
            Welcome,
        }

        public delegate void PacketHandler(NetDataReader Reader);
        public static Dictionary<int, PacketHandler> s_packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)PacketType.Welcome, ClientHandle.Welcome },
        };

        public static void ExecutePacketEvent(int PacketID, NetDataReader Reader)
        {
            PacketHandler Handle;
            if (s_packetHandlers.TryGetValue(PacketID, out Handle))
            {
                Handle(Reader);
            }
        }

        public NetPacketProcessor m_PacketProcessor = new NetPacketProcessor();
        public EventBasedNetListener m_Listener;
        public NetManager m_Instance;
        public NetPeer m_HostEndPoint;

        public bool m_IsReady = false;


        public Client() 
        {
            m_Listener = new EventBasedNetListener();
            m_Instance = new NetManager(m_Listener);
        }

        public void SendToHost(NetDataWriter writer)
        {
            if (m_Instance != null)
            {
                m_HostEndPoint.Send(writer, DeliveryMethod.ReliableOrdered);
            }
        }

        public void ConnectToServer(string address)
        {
            int Port = m_ConnectPort;
            string IP = "127.0.0.1";

            if (!string.IsNullOrEmpty(address))
            {
                if (address.Contains(":"))
                {
                    char seperator = Convert.ToChar(":");

                    string[] sliced = address.Split(seperator);
                    IP = sliced[0];
                    Port = int.Parse(sliced[1]);
                } else
                {
                    IP = address;
                    Port = m_ConnectPort;
                }
            }
            ConnectToServer(IP, Port);
        }

        public void ConnectToServer(string ip, int port, string key = "key")
        {
            Logger.Log($"Connect to {ip}:{port} with key: {key}");

            m_Instance.Start();
            m_Instance.Connect(ip, port, key);
            m_IsReady = true;

            m_Listener.NetworkReceiveEvent += (fromPeer, dataReader, channel, deliveryMethod) =>
            {
                m_HostEndPoint = fromPeer;
                int PacketID = dataReader.GetInt();

                ExecutePacketEvent(PacketID, dataReader);

                dataReader.Recycle();
            };
        }
    }
}

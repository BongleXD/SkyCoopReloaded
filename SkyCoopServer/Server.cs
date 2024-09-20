using LiteNetLib;
using LiteNetLib.Utils;

namespace SkyCoopServer
{
    public class GameServer
    {
        public EventBasedNetListener m_Listener;
        public NetManager m_Instance;
        public bool m_IsReady = false;

        public GameServer()
        {
            m_Listener = new EventBasedNetListener();
            m_Instance = new NetManager(m_Listener);
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
                Console.WriteLine("We got connection: {0}", peer);  // Show peer ip
                NetDataWriter writer = new NetDataWriter();         // Create writer class
                writer.Put("Hello client!");                        // Put some string
                peer.Send(writer, DeliveryMethod.ReliableOrdered);  // Send with reliability
            };
            m_IsReady = true;
            Console.WriteLine($"Server is started port={port}");
        }

    }
}

using LiteNetLib;
using LiteNetLib.Utils;

namespace SkyCoopServer
{
    public class GameServer
    {
        public EventBasedNetListener ServerListener;
        public NetManager Server;
        public bool ServerIsStart = false;

        public GameServer()
        {
            ServerListener = new EventBasedNetListener();
            Server = new NetManager(ServerListener);
        }

        public void StartServer(int port, int maxPlayers, string key = "key")
        {
            Console.WriteLine("Starting server");
            Server.Start(port);

            ServerListener.ConnectionRequestEvent += request =>
            {
                if (Server.ConnectedPeersCount < maxPlayers)
                    request.AcceptIfKey(key);
                else
                    request.Reject();
            };

            ServerListener.PeerConnectedEvent += peer =>
            {
                Console.WriteLine("We got connection: {0}", peer);  // Show peer ip
                NetDataWriter writer = new NetDataWriter();         // Create writer class
                writer.Put("Hello client!");                        // Put some string
                peer.Send(writer, DeliveryMethod.ReliableOrdered);  // Send with reliability
            };
            ServerIsStart = true;
            Console.WriteLine($"Server is started port={port}");
        }

    }
}

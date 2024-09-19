using LiteNetLib;
using MelonLoader;

namespace SkyCoop
{
    public class GameClient
    {
        public EventBasedNetListener ClientListener;
        public NetManager Client;
        public bool ClientIsStart = false;
        public GameClient() 
        {
            ClientListener = new EventBasedNetListener();
            Client = new NetManager(ClientListener);
        }

        public void ConnectToServer(string ip, int port, string key = "key")
        {
            MelonLogger.Msg($"Connect to {ip}{port} with key: {key}");

            Client.Start();
            Client.Connect(ip, port, key);

            ClientListener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) =>
            {
                Console.WriteLine("We got: {0}", dataReader.GetString(100 /* max length of string */));
                dataReader.Recycle();
            };
        }
    }
}

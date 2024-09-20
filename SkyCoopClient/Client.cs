using LiteNetLib;
using MelonLoader;

namespace SkyCoop
{
    public class GameClient
    {
        public EventBasedNetListener m_Listener;
        public NetManager m_Instance;
        public bool m_IsReady = false;
        public GameClient() 
        {
            m_Listener = new EventBasedNetListener();
            m_Instance = new NetManager(m_Listener);
        }

        public void ConnectToServer(string ip, int port, string key = "key")
        {
            Logger.Log($"Connect to {ip}{port} with key: {key}");

            m_Instance.Start();
            m_Instance.Connect(ip, port, key);

            m_Listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) =>
            {
                Logger.Log("We got: "+ dataReader.GetString(100 /* max length of string */));
                dataReader.Recycle();
            };
        }
    }
}

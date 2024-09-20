
namespace SkyCoopServer
{
    public class ServerMain
    {
        public GameServer GameServer;

        public ServerMain()
        {
            GameServer = new GameServer();
        }

        public void Update()
        {
            if (GameServer.m_IsReady)
            {
                GameServer.m_Instance.PollEvents();
            }
        }
    }
}
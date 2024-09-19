
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
            if (GameServer.ServerIsStart)
            {
                GameServer.Server.PollEvents();
            }
        }
    }
}
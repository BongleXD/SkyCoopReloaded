
namespace SkyCoopServer
{
    public class ServerMain
    {
        public Server m_Server;

        public ServerMain()
        {
            m_Server = new Server();
        }

        public void Update()
        {
            if (m_Server.m_IsReady)
            {
                m_Server.m_Instance.PollEvents();
            }
        }
    }
}
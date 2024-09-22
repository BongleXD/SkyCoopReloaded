using MelonLoader;
using SkyCoopServer;

namespace SkyCoop
{
    internal sealed class ModMain : MelonMod
    {
        public static ServerMain Server;
        public static Client Client;

        public override void OnInitializeMelon()
        {
            Server = new ServerMain();
            Client = new Client();
        }

        [Obsolete]
        public override void OnApplicationStart()
        {
            Comps.RegisterComponents();
        }

        public override void OnUpdate()
        {
            if (Client != null && Client.m_IsReady)
            {
                Client.m_Instance.PollEvents();
            }

            if(Server != null && Server.m_Server.m_IsReady)
            {
                Server.Update();
            }
        }
    }
}

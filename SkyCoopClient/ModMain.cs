using MelonLoader;
using SkyCoopServer;

namespace SkyCoop
{
    internal sealed class ModMain : MelonMod
    {
        public static ServerMain Server;
        public static GameClient Client;

        public override void OnInitializeMelon()
        {
            Server = new ServerMain();
            Client = new GameClient();
        }

        [Obsolete]
        public override void OnApplicationStart()
        {
            Comps.RegisterComponents();
        }

        public override void OnUpdate()
        {
            if (Client != null && Client.ClientIsStart)
            {
                Client.Client.PollEvents();
            }

            if(Server != null)
            {
                Server.Update();
            }
        }
    }
}

using MelonLoader;
using SkyCoopServer;
using UnityEngine;

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

        public static void SetAppBackgroundMode()
        {
            if (Application.runInBackground == false)
            {
                Application.runInBackground = true; // Always running in bg
            }
        }

        [Obsolete]
        public override void OnApplicationStart()
        {
            Comps.RegisterComponents();
        }

        public override void OnUpdate()
        {
            SetAppBackgroundMode();
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

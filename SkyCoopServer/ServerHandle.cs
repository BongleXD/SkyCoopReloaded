using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCoopServer
{
    public class ServerHandle
    {
        public static void Welcome(NetPeer Client, NetDataReader Reader)
        {
            int MessageLength = Reader.GetInt();
            string Message = Reader.GetString(MessageLength);
            Console.WriteLine("Сlient "+ Client .Id+ " responced with message: " + Message);
            // TODO : Send server info data here.
        }
    }
}

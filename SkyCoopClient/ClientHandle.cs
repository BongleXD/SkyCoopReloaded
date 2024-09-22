using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using MelonLoader;

namespace SkyCoop
{
    public class ClientHandle
    {
        public static void Welcome(NetDataReader Reader)
        {
            int MessageLength = Reader.GetInt();
            string Message = Reader.GetString(MessageLength);
            Logger.Log(ConsoleColor.Cyan,"Server welcomes me with message: "+ Message);
            ClientSend.Welcome();
            MenuHook.RemovePleaseWait();
            MenuHook.DoOKMessage("Connected!", Message);
        }
    }
}

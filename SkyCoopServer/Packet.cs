using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCoopServer
{
    public class Packet
    {
        public enum Type
        {
            Welcome = 0,
            CFG,
            ClientPosition,
            ClientRotation,
            ClientScene,
        }
    }
}

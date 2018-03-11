using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClient
{
    public struct NetHost
    {
        public IPAddress IP { get; set; }
        public ushort Port { get; set; }

        public NetHost(IPAddress ip, ushort port)
        {
            IP = ip;
            Port = port;
        }
    }
}

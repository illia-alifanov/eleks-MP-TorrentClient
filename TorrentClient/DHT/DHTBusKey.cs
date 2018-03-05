using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClient.DHT
{
    public class DHTBusKey
    {
        public IPAddress IP { get; set; }
        public int Port { get; set; }

        public DHTBusKey(IPAddress ip, int port)
        {
            IP = ip;
            Port = port;
        }
    }
}

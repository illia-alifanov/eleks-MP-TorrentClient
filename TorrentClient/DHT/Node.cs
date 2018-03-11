using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClient.DHT
{
    public struct Node
    {
        public Hash ID { get; set; }
        public NetHost Host { get; set; }

        public Node(Hash id, NetHost host)
        {
            this.ID = id;
            this.Host = host;
        }
    }
}

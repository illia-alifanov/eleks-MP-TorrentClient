using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClient
{
    public class Peer
    {
        public NetHost Host { get; set; }

        public Peer()
        {
        }
        public Peer(NetHost host)
        {
            this.Host = host;
        }
    }
}

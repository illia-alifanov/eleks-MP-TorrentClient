using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClient
{
    public class Peer
    {
        public byte[] IP { get; set; }
        public byte[] Port { get; set; }

        public Peer(byte[] ip, byte[] port)
        {
            this.IP = ip;
            this.Port = port;
        }
    }
}

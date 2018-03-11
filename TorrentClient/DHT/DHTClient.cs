using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClient.DHT
{
    public class DHTClient : UdpClient
    {
        public DHTClient() : base()
        {
            base.Client.ReceiveTimeout = TimeSpan.FromSeconds(5).Seconds;
        }
    }
}

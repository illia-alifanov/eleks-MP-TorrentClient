using System;
using System.Collections.Generic;
using System.Linq;
using TorrentClient.DHT;

namespace TorrentClient
{
    class Program
    {
        static void Main(string[] args)
        {
            DHTTest dht = new DHTTest();
            string nodeId = dht.Ping();

            if (!string.IsNullOrEmpty(nodeId))
            {
                dht.GetPeers();
            }
        }
    }
}

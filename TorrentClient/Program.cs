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
            var torrent = new Torrent()
            {
                Info_Hash = new Hash(System.Text.Encoding.UTF8.GetBytes("04d116664ff21b8336a9988f5f00c164046b4f95"))
            };
            var dhtEngine = new DHTEngine();
            dhtEngine.FindPeers(torrent);

            foreach (var i in torrent.Nodes)
            {
                Console.WriteLine(string.Format("Node id:{0} ", i.Key));
            }
        }
    }
}

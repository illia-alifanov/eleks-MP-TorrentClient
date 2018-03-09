using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TorrentClient.DHT;

namespace TorrentClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string torrent_info = "E58A5149F542B1090315345D7231B8503B46ECB1";

            var torrent = new Torrent()
            {
                Info = torrent_info
            };

            var dhtEngine = new DHTEngine(torrent);
            dhtEngine.FindPeers();

            foreach (var i in dhtEngine.Peers)
            {
                Console.WriteLine(string.Format("Peer IP:{0} ", i.IP));
            }

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}

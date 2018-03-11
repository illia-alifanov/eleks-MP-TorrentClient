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
            //string torrent_info = "E58A5149F542B1090315345D7231B8503B46ECB1";
            string torrent_info = "51B0E1CC4E91C7B08D533EEDA417AE6F56101908";
            
            var torrent = new Torrent(torrent_info);

            var dhtEngine = new DHTEngine(torrent);
            dhtEngine.FindPeers();
            //dhtEngine.FindPeersWithServer();

            foreach (var i in dhtEngine.Peers)
            {
                Console.WriteLine(string.Format("Peer: {0}:{1} ", i.Host.IP, i.Host.Port));
            }

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}

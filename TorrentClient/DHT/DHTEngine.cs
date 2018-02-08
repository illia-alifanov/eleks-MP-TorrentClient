
using BencodeNET.Objects;
using System;

namespace TorrentClient.DHT
{
    public class DHTEngine
    {
        public void FindPeers(Torrent torrent)
        {
            DHT dht = new DHT();
            while (true)
            {
                var response = dht.GetPeers(torrent.Info_Hash);
                if (response != null)
                {
                    foreach (var e in (BDictionary)response)
                    {
                        Console.WriteLine("     GetPeers response " + e.Key + ", Type: " + e.Value.GetType() + ", value: " + e.Value);
                    }
                }
                break;
            }
        }
    }
}

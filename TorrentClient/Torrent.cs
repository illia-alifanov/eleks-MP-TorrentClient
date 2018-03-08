using System.Collections.Generic;
using TorrentClient.DHT;

namespace TorrentClient
{
    public class Torrent
    {
        public Hash Info_Hash { get; set; }

        public string Info { get; set; }
    }
}

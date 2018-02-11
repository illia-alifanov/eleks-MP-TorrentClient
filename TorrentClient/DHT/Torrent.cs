using System.Collections;
using System.Collections.Generic;

namespace TorrentClient.DHT
{
    public class Torrent
    {
        public Hash Info_Hash { get; set; }

        public SortedList<Hash, Node> Nodes { get; set; }

        public Torrent()
        {
            Nodes = new SortedList<Hash, Node>();
        }
    }
}

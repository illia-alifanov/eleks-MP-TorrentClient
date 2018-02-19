using System.Collections;
using System.Collections.Generic;
using TorrentClient.DHT;

namespace TorrentClient
{
    public class Torrent
    {
        public Hash Info_Hash { get; set; }

        public SortedList<Hash, Node> Nodes { get; set; }
        public List<Peer> Peers { get; set; }

        public Torrent()
        {
            Nodes = new SortedList<Hash, Node>();
            Peers = new List<Peer>();
        }
    }
}

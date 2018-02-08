
namespace TorrentClient.DHT
{
    public class Torrent
    {
        public string Info_Hash { get; set; }

        public NodesCollection Nodes { get; set; }
    }
}

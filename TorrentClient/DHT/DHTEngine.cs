namespace TorrentClient.DHT
{
    public class DHTEngine
    {
        public void FindPeers(Torrent torrent)
        {
            DHT dht = new DHT();
            while (torrent.Peers.Count == 0)
            {
                dht.FindPeers(torrent);
            }
        }
    }
}

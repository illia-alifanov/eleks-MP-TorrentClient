using System.Collections.Generic;

namespace TorrentClient.DataStore
{
    public interface ITorrentStore
    {
        void SavePeers();

        HashSet<Peer> ReadPeers();
    }
}

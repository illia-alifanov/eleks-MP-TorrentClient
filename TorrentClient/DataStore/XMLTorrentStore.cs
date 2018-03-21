using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClient.DataStore
{
    public class XMLTorrentStore : ITorrentStore
    {
        private HashSet<Peer> _peers;
        private string _torrentId;
        public XMLTorrentStore(string torrentId, HashSet<Peer> peers)
        {
            _peers = peers;
            _torrentId = torrentId;
        }
        public void SavePeers()
        {
            XMLWriter.WriteXML(_peers, _torrentId);
        }
    }
}

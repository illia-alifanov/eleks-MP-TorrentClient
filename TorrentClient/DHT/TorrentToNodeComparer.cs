using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClient.DHT
{
    public class TorrentToNodeComparer : IComparer<Hash>
    {
        private Torrent _torrent;

        public TorrentToNodeComparer(Torrent torrent)
        {
            _torrent = torrent;
        }

        public int Compare(Hash x, Hash y)
        {
            BitArray torrentID = new BitArray(_torrent.Info_Hash.Value);

            BitArray firstID = new BitArray(x.Value);
            BitArray secondID = new BitArray(y.Value);

            BitArray firstDist = firstID.Xor(torrentID);
            BitArray secondDist = secondID.Xor(torrentID);
            
            // implementation for Big-endian (network byte order) from biggest bytes to smaller
            // find first difference between two distances (bit arrays with 20bytes)
            for (int i = 0; i < 160; i++)
            {
                if (firstDist[i] == secondDist[i]) continue;
                return firstDist[i] ? 1 : -1;
            }
            return 0;
        }
    }
}

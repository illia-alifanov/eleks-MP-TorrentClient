
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;


namespace TorrentClient.DHT
{
    public class NodesCollection : SortedKeyedCollection<BitArray, Node>
    {
        protected override BitArray GetKeyForItem(Node item)
        {
            return item.ID;
        }
    }
}

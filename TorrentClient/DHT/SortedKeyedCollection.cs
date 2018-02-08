using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TorrentClient.DHT
{
    public abstract class SortedKeyedCollection<TKey, TItem> : KeyedCollection<TKey, TItem> where TItem: IComparable
    {
        protected override void InsertItem(int index, TItem item)
        {
            int insertIndex = index;

            for (int i = 0; i < Count; i++)
            {
                TItem retrievedItem = this[i];
                if (item.CompareTo(retrievedItem) < 0)
                {
                    insertIndex = i;
                    break;
                }
            }

            base.InsertItem(insertIndex, item);
        }
    }
}

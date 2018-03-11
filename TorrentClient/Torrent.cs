using System;
using System.Collections.Generic;
using TorrentClient.DHT;

namespace TorrentClient
{
    public class Torrent
    {
        public Hash Info_Hash { get; set; }

        public string Info { get; set; }

        public Torrent()
        {
        }

        public Torrent(string info)
        {
            Info = info;
            Info_Hash = new Hash(GetInfoHash());
        }


        private byte[] GetInfoHash()
        {
            return  ConvertHelper.StringToByteArray(this.Info);
        }
    }
}

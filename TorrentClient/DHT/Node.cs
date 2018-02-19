using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClient.DHT
{
    public struct Node: IComparable
    {
        public Hash ID { get; set; }
        public IPAddress IP { get; set; }
        public ushort Port { get; set; }

        public Node(Hash id, IPAddress ip, ushort port)
        {
            this.ID = id;
            this.IP = ip;
            this.Port = port;
        }

        //public Node(string id)
        //{
        //    this.ID = new BitArray(Encoding.UTF8.GetBytes(id));
        //}

        public int CompareTo(object obj)
        {
            return ID.CompareTo(obj);
        }

        
    }
}

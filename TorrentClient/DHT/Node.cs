using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClient.DHT
{
    public struct Node: IComparable
    {
        public BitArray ID { get; private set; }
        public BitArray IP { get; set; }
        public BitArray Port { get; set; }

        public Node(byte [] id, byte[] ip, byte[] port)
        {
            this.ID = new BitArray(id);
            this.IP = new BitArray(ip);
            this.Port = new BitArray(port);
        }

        //public Node(string id)
        //{
        //    this.ID = new BitArray(Encoding.UTF8.GetBytes(id));
        //}

        public void SetId(BitArray id)
        {
            this.ID = id;
        }
        public void SetId(byte[] id)
        {
            this.ID = new BitArray(id);
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            Node otherNode = (Node)obj;
            BitArray distance = this.ID.Xor(otherNode.ID);

            if (distance.Length > 32)
                throw new ArgumentException("Argument length must be at most 32 bits.");

            int[] array = new int[1];
            distance.CopyTo(array, 0);
            return array[0];
        }

        
    }
}

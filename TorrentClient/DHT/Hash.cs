using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClient.DHT
{
    public class Hash : IComparable
    {
        public byte[] Value { get; set; }

        public Hash()
        {
        }

        public Hash(byte[] value)
        {
            Value = value;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            Hash otherObj = (Hash)obj;

            BitArray thisID = new BitArray(this.Value);
            BitArray otherID = new BitArray(otherObj.Value);
            BitArray distance = thisID.Xor(otherID);
            

            int value = 0;

            for (int i = 0; i < distance.Count; i++)
            {
                if (distance[i])
                    value += Convert.ToInt16(Math.Pow(2, i));
            }

            return value;
        }

        public override string ToString()
        {
            return System.Text.Encoding.UTF8.GetString(Value);
        }
    }
}

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

            // implementation for Big-endian (network byte order) from biggest bytes to smaller
            // instead of XOR we can find first difference between two hashes (bit arrays with 20bytes)
            for (int i = 0; i < 160; i++)
            {
                if (thisID[i] == otherID[i]) continue;
                return thisID[i] ? 1 : -1;
            }
            return 0;
        }

        public override string ToString()
        {
            return System.Text.Encoding.UTF8.GetString(Value);
        }
    }
}

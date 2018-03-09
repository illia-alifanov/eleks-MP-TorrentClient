using System;
using System.Collections;
using System.Text;

namespace TorrentClient
{
    public static class ConvertHelper
    {
        public static byte[] BitArrayToByteArray(BitArray bits)
        {
            byte[] array = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(array, 0);
            return array;
        }

        public static string BitArrayToBitString(BitArray bits)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < bits.Count; i++)
            {
                char c = bits[i] ? '1' : '0';
                sb.Append(c);
            }

            return sb.ToString();
        }

        public static byte[] StringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}

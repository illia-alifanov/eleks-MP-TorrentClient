using System;
using System.Collections;


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
    }
}

using Lab_2.Business;
using Lab_3.Enums;
using System;
using System.Linq;

namespace Lab_3.Models
{
    public static class ByteArrayExtensions
    {
        internal static void XorWith(
            this Byte[] array,
            Byte[] xorArray,
            int inStartIndex,
            int xorStartIndex,
            int length)
        {
            for (int i = 0;  i < length; ++i)
            {
                array[i + inStartIndex] ^= xorArray[i + xorStartIndex];
            }
        }
    }
}

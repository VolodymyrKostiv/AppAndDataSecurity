using Lab_2.Business;
using Lab_3.Enums;
using System;
using System.Linq;

namespace Lab_3.Helpers
{
    public static class ArraysHelper
    {
        public static T[] ConcatArrays<T>(params T[][] arrays)
        {
            var position = 0;
            var outputArray = new T[arrays.Sum(a => a.Length)];

            foreach (var a in arrays)
            {
                Array.Copy(a, 0, outputArray, position, a.Length);
                position += a.Length;
            }

            return outputArray;
        }

        public static void Xor(byte[] array, byte[] xorArray, int inStartIndex, int xorStartIndex, int length)
        {
            for (int i = 0; i < length; ++i)
            {
                array[i + inStartIndex] ^= xorArray[i + xorStartIndex];
            }
        }

        public static byte[] GetMD5HashedKeyForRC5(this byte[] key, KeyLength keyLengthInBytes)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var hasher = new MD5();
            var bytesHash = hasher.HashArrayDigest(key).ToByteArray();

            if (keyLengthInBytes == KeyLength.Bytes_8 || keyLengthInBytes == KeyLength.Bytes_16)
            {
                bytesHash = bytesHash.Take(bytesHash.Length / 2).ToArray();
            }
            else if (keyLengthInBytes == KeyLength.Bytes_32)
            {
                bytesHash = bytesHash
                    .Concat(hasher.HashArrayDigest(bytesHash).ToByteArray())
                    .ToArray();
            }

            if (bytesHash.Length != (int)keyLengthInBytes)
            {
                throw new InvalidOperationException(
                    $"Internal error at {nameof(GetMD5HashedKeyForRC5)} method, " +
                    $"hash result is not equal to {(int)keyLengthInBytes}.");
            }

            return bytesHash;
        }
    }
}

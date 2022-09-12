using Lab_1.Models;
using Lab_3.Constants;
using Lab_3.Helpers;
using Lab_3.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab_3.Models.AlgorithmImplementations
{
    internal class RC5_64Bit : RC5_Base, IRC5Algorithm
    {
        #region prop

        protected override int BytesPerWord { get => sizeof(ulong); }
        private ulong P = RC5Constants.P64;
        private ulong Q = RC5Constants.Q64;

        #endregion fields

        #region constructors

        public RC5_64Bit()
        {
            _numberGenerator = new NumberGenerator();
        }

        #endregion

        #region implementations

        public byte[] EncipherCBCPAD(byte[] input, int numOfRounds, byte[] key)
        {
            var paddedBytes = ArraysHelper.ConcatArrays(input, GetPadding(input));
            int bytesPerBlock = BytesPerBlock;
            ulong[] s = BuildExpandedKeyTable(key, numOfRounds);
            var cnPrev = GetRandomBytesForInitVector().Take(bytesPerBlock).ToArray();
            var encodedFileContent = new byte[cnPrev.Length + paddedBytes.Length];

            EncipherECB(cnPrev, encodedFileContent, inStart: 0, outStart: 0, s, numOfRounds);

            for (int i = 0; i < paddedBytes.Length; i += bytesPerBlock)
            {
                var cn = new byte[bytesPerBlock];
                Array.Copy(paddedBytes, i, cn, 0, cn.Length);

                cn.XorWith(cnPrev, 0, 0, cn.Length);

                EncipherECB(cn, encodedFileContent, 0, i + bytesPerBlock, s, numOfRounds);
                Array.Copy(encodedFileContent, i + bytesPerBlock, cnPrev, 0, cn.Length);
            }

            return encodedFileContent;
        }

        public byte[] DecipherCBCPAD(byte[] input, int numOfRounds, byte[] key)
        {
            var bytesPerBlock = BytesPerBlock;
            ulong[] s = BuildExpandedKeyTable(key, numOfRounds);
            var cnPrev = new byte[bytesPerBlock];
            var decodedFileContent = new byte[input.Length - cnPrev.Length];

            DecipherECB(input, cnPrev, 0, 0, s, numOfRounds);

            for (int i = bytesPerBlock; i < input.Length; i += bytesPerBlock)
            {
                var cn = new byte[bytesPerBlock];
                Array.Copy(input, i, cn, 0, cn.Length);

                DecipherECB(cn, decodedFileContent, 0, i - bytesPerBlock, s, numOfRounds);

                decodedFileContent.XorWith(cnPrev, i - bytesPerBlock, 0, cn.Length);
                Array.Copy(input, i, cnPrev, 0, cnPrev.Length);
            }

            var decodedWithoutPadding = new byte[decodedFileContent.Length - decodedFileContent.Last()];
            Array.Copy(decodedFileContent, decodedWithoutPadding, decodedWithoutPadding.Length);

            return decodedWithoutPadding;
        }

        #endregion implementations

        #region methods

        private void EncipherECB(byte[] inBytes, byte[] outBytes, int inStart, int outStart, ulong[] s, int rounds)
        {
            ulong a = CreateFromBytes(inBytes, inStart);
            ulong b = CreateFromBytes(inBytes, inStart + BytesPerWord);

            a += s[0];
            b += s[1];

            for (int i = 1; i < rounds + 1; ++i)
            {
                a ^= b;
                a = ROL(a, (int)b);
                a += s[2 * i];

                b ^= a;
                b = ROL(b, (int)a);
                b += s[2 * i + 1];
            }

            FillBytesArray(outBytes, outStart, a);
            FillBytesArray(outBytes, outStart + BytesPerWord, a);
        }

        private void DecipherECB(byte[] inBuf, byte[] outBuf, int inStart, int outStart, ulong[] s, int rounds)
        {
            var a = CreateFromBytes(inBuf, inStart);
            var b = CreateFromBytes(inBuf, inStart + BytesPerWord);

            for (var i = rounds; i > 0; --i)
            {
                b -= s[2 * i + 1];
                b = ROR(b, (int)a);
                b ^= a;

                a -= s[2 * i];
                a = ROR(a, (int)b);
                a ^= b;
            }

            a -= s[0];
            b -= s[1];

            FillBytesArray(outBuf, outStart, a);
            FillBytesArray(outBuf, outStart + BytesPerWord, b);
        }

        private byte[] GetPadding(byte[] inBytes)
        {
            var paddingLength = BytesPerBlock - (inBytes.Length % BytesPerBlock);

            var padding = new byte[paddingLength];

            for (int i = 0; i < padding.Length; ++i)
            {
                padding[i] = (byte)paddingLength;
            }

            return padding;
        }

        private byte[] GetRandomBytesForInitVector()
        {
            var ivParts = new List<byte[]>();

            while (ivParts.Sum(ivp => ivp.Length) < BytesPerBlock)
            {
                ivParts.Add(BitConverter.GetBytes(_numberGenerator.GenerateNextNumber()));
            }

            return ArraysHelper.ConcatArrays(ivParts.ToArray());
        }

        private ulong[] BuildExpandedKeyTable(byte[] key, int rounds)
        {
            int keysWordArrLength = key.Length % BytesPerWord > 0
                ? key.Length / BytesPerWord + 1
                : key.Length / BytesPerWord;

            ulong[] lArr = new ulong[keysWordArrLength];

            for (var i = key.Length - 1; i >= 0; i--)
            {
                lArr[i / BytesPerWord] = ROL(lArr[i / BytesPerWord], BitConstants.BitsPerByte);
                lArr[i / BytesPerWord] += key[i];
            }

            ulong[] sArray = new ulong[2 * (rounds + 1)];
            sArray[0] = P;
            var q = Q;

            for (var i = 1; i < sArray.Length; i++)
            {
                sArray[i] = sArray[i - 1];
                sArray[i] += q;
            }

            ulong x = 0, y = 0;
            int n = 3 * Math.Max(sArray.Length, lArr.Length);

            for (int k = 0, i = 0, j = 0; k < n; ++k)
            {
                sArray[i] += x;
                sArray[i] += y;
                sArray[i] = ROL(sArray[i], 3);
                x = sArray[i];

                lArr[j] += x;
                lArr[j] += y;
                lArr[j] = ROL(lArr[j], (int)(x + y));
                y = lArr[j];

                i = (i + 1) % sArray.Length;
                j = (j + 1) % lArr.Length;
            }

            return sArray;
        }

        private ulong CreateFromBytes(byte[] bytes, int startFrom)
        {
            ulong value = 0;

            for (var i = startFrom + BytesPerWord - 1; i > startFrom; --i)
            {
                value = (ulong)(value | bytes[i]);
                value = (ulong)(value << BitConstants.BitsPerByte);
            }

            value = (ulong)(value | bytes[startFrom]);

            return value;
        }

        private byte[] FillBytesArray(byte[] bytesToFill, int start, ulong value)
        {
            int i = 0;
            for (; i < BytesPerWord - 1; ++i)
            {
                bytesToFill[start + i] = (byte)(value & BitConstants.ByteMask);
                value = (ulong)(value >> BitConstants.BitsPerByte);
            }

            bytesToFill[start + i] = (byte)(value & BitConstants.ByteMask);

            return bytesToFill;
        }

        private ulong ROL(ulong value, int offset)
        {
            value = (ulong)((value << offset) | (value >> (BitsPerWord - offset)));

            return value;
        }

        private ulong ROR(ulong value, int offset)
        {
            value = (ulong)((value >> offset) | (value << (BitsPerWord - offset)));

            return value;
        }

        #endregion methods
    }
}

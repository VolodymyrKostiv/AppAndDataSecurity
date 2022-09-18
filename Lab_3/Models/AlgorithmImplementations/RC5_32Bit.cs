using Lab_1.Models;
using Lab_3.Constants;
using Lab_3.Helpers;
using Lab_3.Interfaces;
using System;
using System.Diagnostics;
using System.Linq;

namespace Lab_3.Models.AlgorithmImplementations
{
    internal class RC5_32Bit : RC5_Base, IRC5Algorithm
    {
        #region prop

        protected override int BytesPerWord { get => sizeof(uint); }
        private uint P = RC5Constants.P32;
        private uint Q = RC5Constants.Q32;

        #endregion fields

        #region constructors

        public RC5_32Bit()
        {
            _numberGenerator = new NumberGenerator();
        }

        #endregion

        #region implementations

        public byte[] EncipherCBCPAD(string fileName, int numOfRounds, byte[] key)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            _inputFileHelper.OpenFile(fileName);
            _outputFileHelper.OpenFile(fileName + "_encrypted");

            uint[] S = BuildExpandedKeyTable(key, numOfRounds);
            int bytesPerBlock = BytesPerBlock;

            byte[] cnPrev = GetRandomBytesForInitVector().Take(bytesPerBlock).ToArray();
            byte[] bytesToEncode = new byte[bytesPerBlock];
            byte[] encodedBlock = new byte[bytesPerBlock];

            EncipherECB(S, numOfRounds, cnPrev, encodedBlock);
            _outputFileHelper.WriteBlock(encodedBlock);

            bool endOfFile = false;

            do
            {
                bytesToEncode = _inputFileHelper.ReadBlock(bytesPerBlock);
                if (bytesToEncode.Length < bytesPerBlock)
                {
                    endOfFile = true;
                    bytesToEncode = ArraysHelper.ConcatArrays(bytesToEncode, GetPadding(bytesToEncode));
                }

                bytesToEncode.XorWith(cnPrev, 0, 0, cnPrev.Length);

                EncipherECB(S, numOfRounds, bytesToEncode, encodedBlock);
                Array.Copy(encodedBlock, cnPrev, encodedBlock.Length);

                _outputFileHelper.WriteBlock(encodedBlock);

            } while (!endOfFile);

            _inputFileHelper.CloseFile();
            _outputFileHelper.CloseFile();

            watch.Stop();

            var inputSec = _inputFileHelper.Watch.Elapsed.TotalSeconds;
            var outputSec = _outputFileHelper.Watch.Elapsed.TotalSeconds;
            var total = watch.Elapsed.TotalSeconds; 

            return encodedBlock;
        }

        public byte[] DecipherCBCPAD(string fileName, int numOfRounds, byte[] key)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            _inputFileHelper.OpenFile(fileName);
            _outputFileHelper.OpenFile(fileName + "_decrypted");

            uint[] S = BuildExpandedKeyTable(key, numOfRounds);
            int bytesPerBlock = BytesPerBlock;

            byte[] cnPrev = new byte[bytesPerBlock];
            byte[] bytesToDecode = _inputFileHelper.ReadBlock(bytesPerBlock);
            byte[] decodedBlock = new byte[bytesPerBlock];

            DecipherECB(S, numOfRounds, bytesToDecode, decodedBlock);
            Array.Copy(decodedBlock, cnPrev, bytesToDecode.Length);
            var firstLoop = true;

            do
            {
                bytesToDecode = _inputFileHelper.ReadBlock(bytesPerBlock);
                if (bytesToDecode.Length <= 0 && !firstLoop)
                {
                    _outputFileHelper.WriteBlock(decodedBlock.Take(decodedBlock.Length - decodedBlock.Last()).ToArray());
                    break;
                }
                else if (!firstLoop)
                {
                    _outputFileHelper.WriteBlock(decodedBlock);
                }

                DecipherECB(S, numOfRounds, bytesToDecode, decodedBlock);

                decodedBlock.XorWith(cnPrev, 0, 0, cnPrev.Length);

                Array.Copy(bytesToDecode, cnPrev, bytesToDecode.Length);
                firstLoop = false;
            } while (true);

            _inputFileHelper.CloseFile();
            _outputFileHelper.CloseFile();

            watch.Stop();

            var inputSec = _inputFileHelper.Watch.Elapsed.TotalSeconds;
            var outputSec = _outputFileHelper.Watch.Elapsed.TotalSeconds;
            var total = watch.Elapsed.TotalSeconds;

            return decodedBlock;
        }

        #endregion implementations

        #region methods

        private void EncipherECB(uint[] S, int rounds, byte[] inBytes, byte[] outBytes)
        {
            uint A = CreateFromBytes(inBytes, 0);
            uint B = CreateFromBytes(inBytes, BytesPerWord);

            A += S[0];
            B += S[1];

            for (int i = 1; i < rounds + 1; ++i)
            {
                A ^= B;
                A = ROL(A, (int)B);
                A += S[2 * i];

                B ^= A;
                B = ROL(B, (int)A);
                B += S[2 * i + 1];
            }

            FillBytesArray(outBytes, 0, A);
            FillBytesArray(outBytes, BytesPerWord, B);
        }

        private void DecipherECB(uint[] S, int rounds, byte[] inBytes, byte[] outBytes)
        {
            uint A = CreateFromBytes(inBytes, 0);
            uint B = CreateFromBytes(inBytes, BytesPerWord);

            for (var i = rounds; i > 0; --i)
            {
                B -= S[2 * i + 1];
                B = ROR(B, (int)A);
                B ^= A;

                A -= S[2 * i];
                A = ROR(A,(int)B);
                A ^= B;
            }

            A -= S[0];
            B -= S[1];

            FillBytesArray(outBytes, 0, A);
            FillBytesArray(outBytes, BytesPerWord, B);
        }

        private uint[] BuildExpandedKeyTable(byte[] key, int rounds)
        {
            int keysWordArrLength = key.Length % BytesPerWord > 0
                ? key.Length / BytesPerWord + 1
                : key.Length / BytesPerWord;

            uint[] L = new uint[keysWordArrLength];

            for (int i = key.Length - 1; i >= 0; i--)
            {
                L[i / BytesPerWord] = ROL(L[i / BytesPerWord], BitConstants.BitsPerByte);
                L[i / BytesPerWord] += key[i];
            }

            uint[] S = new uint[2 * (rounds + 1)];
            S[0] = P;

            for (int i = 1; i < S.Length; i++)
            {
                S[i] = S[i - 1];
                S[i] += Q;
            }

            uint x = 0, y = 0;
            int n = 3 * Math.Max(S.Length, L.Length);

            for (int k = 0, i = 0, j = 0; k < n; ++k)
            {
                S[i] += x;
                S[i] += y;
                S[i] = ROL(S[i], 3);
                x = S[i];

                L[j] += x;
                L[j] += y;
                L[j] = ROL(L[j], (int)(x + y));
                y = L[j];

                i = (i + 1) % S.Length;
                j = (j + 1) % L.Length;
            }

            return S;
        }

        private uint CreateFromBytes(byte[] bytes, int startFrom)
        {
            uint value = 0;

            for (int i = startFrom + BytesPerWord - 1; i > startFrom; --i)
            {
                value = (uint)(value | bytes[i]);
                value = (uint)(value << BitConstants.BitsPerByte);
            }

            value = (uint)(value | bytes[startFrom]);

            return value;
        }

        private byte[] FillBytesArray(byte[] bytesToFill, int start, uint value)
        {
            int i = 0;
            for (; i < BytesPerWord - 1; ++i)
            {
                bytesToFill[start + i] = (byte)(value & BitConstants.ByteMask);
                value = (uint)(value >> BitConstants.BitsPerByte);
            }

            bytesToFill[start + i] = (byte)(value & BitConstants.ByteMask);

            return bytesToFill;
        }

        private uint ROL(uint value, int offset)
        {
            //offset %= BytesPerWord;

            value = (uint)((value << offset) | (value >> (BitsPerWord - offset)));

            return value;
        }

        private uint ROR(uint value, int offset)
        {
            //offset %= BytesPerWord;

            value = (uint)((value >> offset) | (value << (BitsPerWord - offset)));

            return value;
        }

        #endregion methods
    }
}

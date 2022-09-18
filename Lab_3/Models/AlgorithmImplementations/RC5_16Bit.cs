using Lab_3.Constants;
using Lab_3.Helpers;
using Lab_3.Interfaces;
using System;
using System.IO;
using System.Linq;

namespace Lab_3.Models.AlgorithmImplementations
{
    internal class RC5_16Bit : RC5_Base, IRC5Algorithm
    {
        #region prop

        protected override int BytesPerWord { get => sizeof(ushort); }
        private ushort P = RC5Constants.P16;
        private ushort Q = RC5Constants.Q16;

        #endregion fields

        #region constructors

        public RC5_16Bit()
        {
            //
        }

        #endregion

        #region implementations

        public byte[] EncipherCBCPAD(string fileName, int numOfRounds, byte[] key)
        {
            _inputFileHelper.OpenFile(fileName);
            _outputFileHelper.OpenFile(fileName + "_encrypted");

            ushort[] S = BuildExpandedKeyTable(key, numOfRounds);
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

            return encodedBlock;
        }

        public byte[] DecipherCBCPAD(string fileName, int numOfRounds, byte[] key)
        {
            _inputFileHelper.OpenFile(fileName);
            _outputFileHelper.OpenFile(fileName + "_decrypted");

            ushort[] S = BuildExpandedKeyTable(key, numOfRounds);
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

            var result = decodedBlock.Take(decodedBlock.Length - decodedBlock.Last()).ToArray();

            return result;
        }

        #endregion implementations

        #region methods

        private void EncipherECB(ushort[] S, int rounds, byte[] inBytes, byte[] outBytes)
        {
            ushort A = CreateFromBytes(inBytes, 0);
            ushort B = CreateFromBytes(inBytes, BytesPerWord);

            A += S[0];
            B += S[1];

            for (int i = 1; i < rounds + 1; ++i)
            {
                A ^= B;
                A = ROL(A, B);
                A += S[2 * i];

                B ^= A;
                B = ROL(B, A);
                B += S[2 * i + 1];
            }

            FillBytesArray(outBytes, 0, A);
            FillBytesArray(outBytes, BytesPerWord, B);
        }

        private void DecipherECB(ushort[] S, int rounds, byte[] inBytes, byte[] outBytes)
        {
            ushort A = CreateFromBytes(inBytes, 0);
            ushort B = CreateFromBytes(inBytes, BytesPerWord);

            for (var i = rounds; i > 0; --i)
            {
                B -= S[2 * i + 1];
                B = ROR(B, A);
                B ^= A;

                A -= S[2 * i];
                A = ROR(A, B);
                A ^= B;
            }

            A -= S[0];
            B -= S[1];

            FillBytesArray(outBytes, 0, A);
            FillBytesArray(outBytes, BytesPerWord, B);
        }

        private ushort[] BuildExpandedKeyTable(byte[] key, int rounds)
        {
            int keysWordArrLength = key.Length % BytesPerWord > 0
                ? key.Length / BytesPerWord + 1
                : key.Length / BytesPerWord;

            ushort[] L = new ushort[keysWordArrLength];

            for (int i = key.Length - 1; i >= 0; i--)
            {
                L[i / BytesPerWord] = ROL(L[i / BytesPerWord], BitConstants.BitsPerByte);
                L[i / BytesPerWord] += key[i];
            }

            ushort[] S = new ushort[2 * (rounds + 1)];
            S[0] = P;

            for (int i = 1; i < S.Length; i++)
            {
                S[i] = S[i - 1];
                S[i] += Q;
            }

            ushort x = 0, y = 0;
            int n = 3 * Math.Max(S.Length, L.Length);

            for (int k = 0, i = 0, j = 0; k < n; ++k)
            {
                S[i] += x;
                S[i] += y;
                S[i] = ROL(S[i], 3);
                x = S[i];

                L[j] += x;
                L[j] += y;
                L[j] = ROL(L[j], x + y);
                y = L[j];

                i = (i + 1) % S.Length;
                j = (j + 1) % L.Length;
            }

            return S;
        }

        private ushort CreateFromBytes(byte[] bytes, int startFrom)
        {
            ushort value = 0;

            for (int i = startFrom + BytesPerWord - 1; i > startFrom; --i)
            {
                value = (ushort)(value | bytes[i]);
                value = (ushort)(value << BitConstants.BitsPerByte);
            }

            value = (ushort)(value | bytes[startFrom]);

            return value;
        }

        private byte[] FillBytesArray(byte[] bytesToFill, int start, ushort value)
        {
            int i = 0;
            for (; i < BytesPerWord - 1; ++i)
            {
                bytesToFill[start + i] = (byte)(value & BitConstants.ByteMask);
                value = (ushort)(value >> BitConstants.BitsPerByte);
            }

            bytesToFill[start + i] = (byte)(value & BitConstants.ByteMask);

            return bytesToFill;
        }

        private ushort ROL(ushort value, int offset)
        {
            offset %= BytesPerWord;

            value = (ushort)((value << offset) | (value >> (BitsPerWord - offset)));

            return value;
        }

        private ushort ROR(ushort value, int offset)
        {
            offset %= BytesPerWord;

            value = (ushort)((value >> offset) | (value << (BitsPerWord - offset)));

            return value;
        }

        #endregion methods
    }
}

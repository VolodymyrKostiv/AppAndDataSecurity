using Lab_2.Constants;
using System;
using System.Text;

namespace Lab_2.Business
{
    public class MD5
    {
        private const int OptimalChunkSizeMultiplier = 100_000;
        private const uint OptimalChunkSize = MD5Constants.BytesCountPerBits512Block * OptimalChunkSizeMultiplier;

        public string HashAsString => Hash.ToString();

        private Digest Hash { get; set; }

        public string HashString(string message)
        {
            var encoding = Encoding.UTF8;
            var byteMessage = encoding.GetBytes(message);

            var hash = CalculateMD5(byteMessage);

            return hash.ToString();
        }

        public string HashArray(byte[] arr)
        {
            var byteMessage = CalculateMD5(arr);

            return byteMessage.ToString();
        }

        public Digest HashArrayDigest(byte[] arr)
        {
            var byteMessage = CalculateMD5(arr);

            return byteMessage;
        }

        private Digest CalculateMD5(byte[] message)
        {
            Hash = new Digest();

            byte[] paddedMessage = CreatePaddedBuffer(message);

            ulong N = (ulong)(paddedMessage.Length * BitConstants.BitsPerByte / MD5Constants.BlockSizeInBits);

            for (uint i = 0; i < N; i++)
            {
                uint[] X = CopyBlock(paddedMessage, i);

                PerformCompression(X);
            }

            return Hash;
        }


        private byte[] CreatePaddedBuffer(byte[] inputMessage)
        {
            uint inputMessageLength = (uint)inputMessage.Length;

            uint pad = CalculatePadding(inputMessageLength);

            uint sizeMsgBuff = inputMessageLength + (pad / BitConstants.BitsPerByte) + MD5Constants.MessageMaxSize;
            ulong sizeMsg = inputMessageLength * BitConstants.BitsPerByte;

            byte[] bMsg = new byte[sizeMsgBuff];

            for (uint i = 0; i < inputMessageLength; i++)
            {
                bMsg[i] = inputMessage[i];
            }

            bMsg[inputMessageLength] |= BitConstants.BitsRepr128;

            for (uint i = BitConstants.BitsPerByte; i > 0; i--)
            {
                bMsg[sizeMsgBuff - i] = (byte)(sizeMsg >> (int)((BitConstants.BitsPerByte - i) * BitConstants.BitsPerByte) & BitConstants.BitsRepr255);
            }

            return bMsg;
        }

        private uint CalculatePadding(uint inputMesLength)
        {
            uint pad = (MD5Constants.MessageToPaddingLength - ((inputMesLength * BitConstants.BitsPerByte) % MD5Constants.BlockSizeInBits));
            pad = (pad + MD5Constants.BlockSizeInBits) % MD5Constants.BlockSizeInBits;

            if (pad == 0)
            {
                pad = MD5Constants.BlockSizeInBits;
            }

            return pad;
        }

        private uint[] CopyBlock(byte[] message, uint block)
        {
            var blockSizeInBytes = MD5Constants.Words32BitArraySize * BitConstants.BytesPer32BitWord;
            var messageStartIndex = block * blockSizeInBytes;
            var extractedArray = new uint[blockSizeInBytes / BitConstants.BytesPer32BitWord];

            for (uint i = 0; i < blockSizeInBytes; i += BitConstants.BytesPer32BitWord)
            {
                var j = messageStartIndex + i;

                extractedArray[i / BitConstants.BytesPer32BitWord] =
                      message[j]
                    | (((uint)message[j + 1]) << ((int)BitConstants.BitsPerByte * 1))
                    | (((uint)message[j + 2]) << ((int)BitConstants.BitsPerByte * 2))
                    | (((uint)message[j + 3]) << ((int)BitConstants.BitsPerByte * 3));
            }

            return extractedArray;
        }

        private void PerformCompression(uint[] X)
        {
            uint F = 0, i = 0, k = 0;
            var blockSize = MD5Constants.BytesCountPerBits512Block;
            var MDq = Hash.Clone();

            RoundFunctions.FirstRound (MDq, blockSize, X, ref F, ref i, ref k);
            RoundFunctions.SecondRound(MDq, blockSize, X, ref F, ref i, ref k);
            RoundFunctions.ThirdRound (MDq, blockSize, X, ref F, ref i, ref k);
            RoundFunctions.FourthRound(MDq, blockSize, X, ref F, ref i, ref k);

            Hash += MDq;
        }

        #region bad ideas
        //internal async Task<Digest> HashFileAsync(string filePath)
        //{
        //    Hash = new Digest();

        //    using (var filestream = File.OpenRead(filePath))
        //    {
        //        ulong totalBytesRead = 0;
        //        int currentBytesRead = 0;
        //        bool fileEnd = false;

        //        do
        //        {
        //            var chunk = new byte[10_000];

        //            currentBytesRead = await filestream.ReadAsync(chunk, 0, chunk.Length);
        //            totalBytesRead += (ulong)currentBytesRead;

        //            if (currentBytesRead < chunk.Length)
        //            {
        //                byte[] lastChunk = null;

        //                if (currentBytesRead == 0)
        //                {
        //                    //lastChunk = ;
        //                }
        //                else
        //                {
        //                    lastChunk = new byte[currentBytesRead];
        //                    Array.Copy(chunk, lastChunk, currentBytesRead);

        //                    //lastChunk = 
        //                }

        //                chunk = lastChunk;
        //                fileEnd = true;
        //            }
        //            for (UInt32 i = 0; i < chunk.Length / MD5Constants.BytesCountPerBits512Block; i++)
        //            {
        //                uint[] X = CopyBlock(chunk, i);

        //                PerformCompression(X);
        //            }
        //        } while(!fileEnd);
        //    }

        //    return Hash;
        //}
        #endregion
    }
}

using Lab_2.Constants;

namespace Lab_2.Helpers
{
    internal class BitsHelper
    {
        public static uint[] Extract32BitWords(byte[] message, uint blockNo, uint blockSizeInBytes)
        {
            var messageStartIndex = blockNo * blockSizeInBytes;
            var extractedArray = new uint[blockSizeInBytes / BitConstants.BytesPer32BitWord];

            for (uint i = 0; i < blockSizeInBytes; i += BitConstants.BytesPer32BitWord)
            {
                var j = messageStartIndex + i;

                extractedArray[i / BitConstants.BytesPer32BitWord] = // form 32-bit word from four bytes
                      message[j]                                                   // first byte
                    | (((uint)message[j + 1]) << ((int)BitConstants.BitsPerByte * 1))  // second byte
                    | (((uint)message[j + 2]) << ((int)BitConstants.BitsPerByte * 2))  // third byte
                    | (((uint)message[j + 3]) << ((int)BitConstants.BitsPerByte * 3)); // fourth byte
            }

            return extractedArray;
        }

        public static uint LeftRotate(uint x, int c)
        {
            return (x << c)
                 | (x >> (int)(BitConstants.BitsPerByte * BitConstants.BytesPer32BitWord - c));
        }

    }
}

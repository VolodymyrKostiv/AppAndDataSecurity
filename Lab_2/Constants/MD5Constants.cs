﻿namespace Lab_2.Constants
{
    internal static class MD5Constants
    {
        public const uint MessageMaxSize = 8u;
        public const uint Words32BitArraySize = 16u;
        public const uint BlockSizeInBits = 512u;
        public const uint MessageToPaddingLength = 448u;

        public const uint BytesCountPerBits512Block = BlockSizeInBits / BitConstants.BitsPerByte;

        public const uint MDBufferA = 0x67452301;
        public const uint MDBufferB = 0xEFCDAB89;
        public const uint MDBufferC = 0x98BADCFE;
        public const uint MDBufferD = 0x10325476;

        public readonly static uint[] T = new uint[]
        {
            0xD76AA478, 0xE8C7B756, 0x242070DB, 0xC1BDCEEE, 0xF57C0FAF, 0x4787C62A, 0xA8304613, 0xFD469501,
            0x698098D8, 0x8B44F7AF, 0xFFFF5BB1, 0x895CD7BE, 0x6B901122, 0xFD987193, 0xA679438E, 0x49B40821,

            0xF61E2562, 0xC040B340, 0x265E5A51, 0xE9B6C7AA, 0xD62F105D, 0x2441453 , 0xD8A1E681, 0xE7D3FBC8,
            0x21E1CDE6, 0xC33707D6, 0xF4D50D87, 0x455A14ED, 0xA9E3E905, 0xFCEFA3F8, 0x676F02D9, 0x8D2A4C8A,

            0xFFFA3942, 0x8771F681, 0x6D9D6122, 0xFDE5380C, 0xA4BEEA44, 0x4BDECFA9, 0xF6BB4B60, 0xBEBFBC70,
            0x289B7EC6, 0xEAA127FA, 0xD4EF3085, 0x4881D05 , 0xD9D4D039, 0xE6DB99E5, 0x1FA27CF8, 0xC4AC5665,
            
            0xF4292244, 0x432AFF97, 0xAB9423A7, 0xFC93A039, 0x655B59C3, 0x8F0CCC92, 0xFFEFF47D, 0x85845DD1,
            0x6fA87E4F, 0xFE2CE6E0, 0xA3014314, 0x4E0811A1, 0xF7537E82, 0xBD3AF235, 0x2AD7D2BB, 0xEB86D391
        };

        public readonly static int[] S = new int[] 
        {
            7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,
            5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,
            4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,
            6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21
        };
    }
}

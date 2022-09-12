namespace Lab_3.Constants
{
    internal static  class RC5Constants
    {
        public const ushort P16 = 0xB7E1;
        public const ushort Q16 = 0x9E37;
        public const uint   P32 = 0xB7E15162;
        public const uint   Q32 = 0x9E3779B9;
        public const ulong  P64 = 0xB7E151628AED2A6B;
        public const ulong  Q64 = 0x9E3779B97F4A7C15;

        public const int MinRoundCount = 0;
        public const int MaxRoundCount = 255;
        public const int MinSecretKeyOctetsCount = 0;
        public const int MaxSecretKeyOctetsCount = 255;
        public const int MinKeySizeInBytes = 0;
        public const int MaxKeySizeInBytes = 2040;
        public const int WordsPerBlock = 2;
    }
}

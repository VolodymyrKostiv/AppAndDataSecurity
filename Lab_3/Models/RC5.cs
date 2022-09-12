using Lab_3.Enums;
using Lab_3.Interfaces;
using Lab_3.Models.AlgorithmImplementations;

namespace Lab_3.Models
{
    public class RC5
    {
        #region fields

        private IRC5Algorithm _algorithm;
        private WordType _wordType;

        #endregion fields

        #region constructors

        public RC5(WordType wordType = WordType.Word32Bits)
        {
            _wordType = wordType;
            ChangeAlgorithm(wordType);
        }

        public void ChangeAlgorithm(WordType wordType)
        {
            switch(wordType)
            {
                case WordType.Word16Bits:
                    _algorithm = new RC5_16Bit();
                    break;
                case WordType.Word32Bits:
                    _algorithm = new RC5_32Bit();
                    break;
                case WordType.Word64Bits:
                    _algorithm = new RC5_64Bit();
                    break;
            }
        }

        public byte[] Encrypt(byte[] input, int numOfRounds, byte[] key)
        {
            return _algorithm.EncipherCBCPAD(input, numOfRounds, key);
        }

        public byte[] Decrypt(byte[] input, int numOfRounds, byte[] key)
        {
            return _algorithm.DecipherCBCPAD(input, numOfRounds, key);
        }


        #endregion constructors
    }
}

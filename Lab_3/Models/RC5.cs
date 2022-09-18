using Lab_3.Enums;
using Lab_3.Interfaces;
using Lab_3.Models.AlgorithmImplementations;
using System.Threading.Tasks;

namespace Lab_3.Models
{
    public class RC5
    {
        #region fields

        private IRC5Algorithm _algorithm;

        #endregion fields

        #region constructors

        public RC5(WordType wordType = WordType.Word_32)
        {
            ChangeAlgorithm(wordType);
        }

        #endregion constructors

        #region methods

        public async Task<byte[]> Encrypt(string fileName, int numOfRounds, byte[] key)
        {
            return _algorithm.EncipherCBCPAD(fileName, numOfRounds, key);
        }

        public async Task<byte[]> Decrypt(string fileName, int numOfRounds, byte[] key)
        {
            return _algorithm.DecipherCBCPAD(fileName, numOfRounds, key);
        }

        public void ChangeAlgorithm(WordType wordType)
        {
            switch (wordType)
            {
                case WordType.Word_16:
                    _algorithm = new RC5_16Bit();
                    break;
                case WordType.Word_32:
                    _algorithm = new RC5_32Bit();
                    break;
                case WordType.Word_64:
                    _algorithm = new RC5_64Bit();
                    break;
            }
        }

        #endregion methods
    }
}

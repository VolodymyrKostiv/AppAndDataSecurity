namespace Lab_1.Models
{
    internal class NumberGenerator
    {
        #region fields

        private readonly ulong _modulus;
        private readonly ulong _multiplier;
        private readonly ulong _increment;

        #endregion fields

        #region constructors

        public NumberGenerator(ulong multiplier, ulong increment, ulong modulus)
        {
            _multiplier = multiplier;
            _increment = increment;
            _modulus = modulus;
        }

        #endregion constructors

        #region methods

        public ulong GenerateNextNumber(ulong _currentNumber)
        {
            return (_multiplier * _currentNumber + _increment) % _modulus;
        }

        #endregion methods
    }
}

using System;

namespace Lab_1.Models
{
    public class NumberGenerator
    {
        #region fields

        private ulong _currentNumber = 31;
        private readonly ulong _modulus = (ulong)Math.Pow(2, 31) - 1;
        private readonly ulong _multiplier = (ulong)Math.Pow(7, 5);
        private readonly ulong _increment = 17711;
        private readonly ulong _startNumber = 31;

        #endregion fields

        #region constructors

        public NumberGenerator()
        {

        }

        public NumberGenerator(ulong multiplier, ulong increment, ulong modulus)
        {
            _multiplier = multiplier;
            _increment = increment;
            _modulus = modulus;
        }

        public NumberGenerator(ulong multiplier, ulong increment, ulong modulus, ulong startNumber)
            : this(multiplier, increment, modulus)
        {
            _startNumber = startNumber;
            _currentNumber = startNumber;
        }

        #endregion constructors

        #region methods

        public ulong GenerateNextNumber()
        {
            var nextNumber = _currentNumber;

            _currentNumber = GenerateNextNumber(nextNumber);

            return nextNumber;
        }

        public ulong GenerateNextNumber(ulong _currentNumber)
        {
            return (_multiplier * _currentNumber + _increment) % _modulus;
        }

        #endregion methods
    }
}

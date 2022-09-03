using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_1.Models
{

    #region delegates 

    public class PeriodFoundEventArgs : EventArgs
    {
        public ulong Period { get; set; }
    }

    public delegate void PeriodFoundEventHandler(object sender, PeriodFoundEventArgs e);

    public class PartOfSequenceGeneratedEventArgs : EventArgs
    {
        public List<ulong> Numbers { get; set; }
    }

    public delegate void PartOfSequenceGeneratedEventHandler(object sender, PartOfSequenceGeneratedEventArgs e);

    public class SequenceGeneratedEventArgs : EventArgs
    {
        public List<ulong> Numbers { get; set; }
    }

    public delegate void SequenceGeneratedEventHandler(object sender, SequenceGeneratedEventArgs e);

    #endregion delegates

    internal class LinearCongruentialGenerator
    {
        #region fields

        private NumberGenerator _numberGenerator;
        private ulong _startValue;
        private ulong _count;

        #endregion fields

        #region constants

        private const int _numbersInSequencePart = 1000;

        #endregion constants

        #region constructors

        public LinearCongruentialGenerator()
        {
        }

        #endregion constructors

        #region events 

        public event PartOfSequenceGeneratedEventHandler PartOfSequenceGenerated;
        public event PeriodFoundEventHandler PeriodFound;
        public event SequenceGeneratedEventHandler SequenceGenerated;

        #endregion events

        #region methods

        public async Task StartGenerator(GeneratorMode mode, ulong count, ulong startValue, ulong multiplier, ulong increment, ulong modulus)
        {
            _count = count;
            _startValue = startValue;
            _numberGenerator = new NumberGenerator(multiplier, increment, modulus);

            switch (mode)
            {
                case GeneratorMode.USE_BUFFER:
                    await GenerateWithBuffer();
                    break;
                case GeneratorMode.DIRECTLY_INTO_FILE:
                    await GenerateIntoFile();
                    break;
                default:
                    return;
            }
        }

        private async Task GenerateWithBuffer()
        {
            ulong currentNumber = _numberGenerator.GenerateNextNumber(_startValue);

            HashSet<ulong> uniqueValues = new HashSet<ulong>();
            uniqueValues.Add(_startValue);
            uniqueValues.Add(currentNumber);

            List<ulong> partSequence = new List<ulong>(_numbersInSequencePart);
            partSequence.Add(_startValue);
            partSequence.Add(currentNumber);

            if (uniqueValues.Count <= 1)
            {
                PeriodFound?.Invoke(this, new PeriodFoundEventArgs() { Period = (ulong)uniqueValues.Count });
            }

            for (ulong i = 0; i < _count - 1; i++)
            {
                ulong previousNumber = currentNumber;
                currentNumber = _numberGenerator.GenerateNextNumber(currentNumber);

                if (uniqueValues.Contains(currentNumber))
                {
                    PeriodFound?.Invoke(this, new PeriodFoundEventArgs() { Period = (ulong)uniqueValues.Count });
                }

                uniqueValues.Add(currentNumber);
                partSequence.Add(currentNumber);

                if (partSequence.Count >= _numbersInSequencePart)
                {
                    PartOfSequenceGenerated?.Invoke(this, new PartOfSequenceGeneratedEventArgs() { Numbers = partSequence });
                    partSequence.Clear();
                }
            }

            SequenceGenerated?.Invoke(this, new SequenceGeneratedEventArgs() { Numbers = partSequence });
        }

        private async Task GenerateIntoFile()
        {
            FileWriter fileWriter = new FileWriter();
            fileWriter.CreateFile(@"D:\University\Term 7\Security\Labs\TestFiles\Lab_1\BigChungus.txt");

            ulong currentNumber = _numberGenerator.GenerateNextNumber(_startValue);

            await fileWriter.WriteIntoFile("0. " + _startValue.ToString() + "\n");
            await fileWriter.WriteIntoFile("1. " + currentNumber.ToString() + "\n");

            ulong firstNumber = _startValue;
            ulong secondNumber = currentNumber;

            if (firstNumber == secondNumber)
            {
                PeriodFound?.Invoke(this, new PeriodFoundEventArgs() { Period = 1 });
            }

            for (ulong i = 0; i < _count - 1; i++)
            {
                ulong previousNumber = currentNumber;
                currentNumber = _numberGenerator.GenerateNextNumber(currentNumber);

                if (DuplicateFound(currentNumber, previousNumber, firstNumber, secondNumber))
                {
                    PeriodFound?.Invoke(this, new PeriodFoundEventArgs() { Period = (i + 2) });
                }

                await fileWriter.WriteIntoFile((i + 2).ToString() + ". "+ currentNumber.ToString() + "\n");
            }

            fileWriter.CloseFile();
        }

        private bool DuplicateFound(ulong current, ulong previous, ulong first, ulong second)
        {
            return (current == previous || current == first || current == second);
        }

        #endregion methods
    }
}

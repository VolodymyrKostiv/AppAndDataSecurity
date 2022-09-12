using Lab_1.Models;
using Lab_3.Constants;

namespace Lab_3.Models
{
    internal abstract class RC5_Base
    {
        protected abstract int BytesPerWord { get; }
        protected int BitsPerWord => BytesPerWord * BitConstants.BitsPerByte; 
        protected int BytesPerBlock => BytesPerWord * RC5Constants.WordsPerBlock;
        protected NumberGenerator _numberGenerator = new NumberGenerator();
    }
}

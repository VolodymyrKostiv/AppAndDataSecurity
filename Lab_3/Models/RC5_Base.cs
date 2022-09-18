using Lab_1.Models;
using Lab_3.Constants;
using Lab_3.Helpers;
using Lab_3.Helpers.FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab_3.Models
{
    internal abstract class RC5_Base
    {
        protected abstract int BytesPerWord { get; }
        protected int BitsPerWord => BytesPerWord * BitConstants.BitsPerByte; 
        protected int BytesPerBlock => BytesPerWord * RC5Constants.WordsPerBlock;
        protected NumberGenerator _numberGenerator = new NumberGenerator();
        protected InputFileHelper _inputFileHelper = new InputFileHelper();
        protected OutputFileHelper _outputFileHelper = new OutputFileHelper();

        protected byte[] GetPadding(byte[] inBytes)
        {
            int paddingLength = BytesPerBlock - (inBytes.Length % BytesPerBlock);

            byte[] padding = new byte[paddingLength];

            for (int i = 0; i < padding.Length; ++i)
            {
                padding[i] = (byte)paddingLength;
            }

            return padding;
        }

        protected byte[] GetRandomBytesForInitVector()
        {
            List<byte[]> ivParts = new List<byte[]>();

            while (ivParts.Sum(ivp => ivp.Length) < BytesPerBlock)
            {
                ivParts.Add(BitConverter.GetBytes(_numberGenerator.GenerateNextNumber()));
            }

            return ArraysHelper.ConcatArrays(ivParts.ToArray());
        }

    }
}

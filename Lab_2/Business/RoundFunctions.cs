namespace Lab_2.Business
{
    internal class RoundFunctions
    {
        public static void FirstRound(Digest MDq, uint blockSize, uint[] X, ref uint F, ref uint i, ref uint k)
        {
            for (i = 0; i < blockSize / 4; ++i)
            {
                k = i;
                F = StepFunctions.FunctionF(MDq.B, MDq.C, MDq.D);

                MDq.IterationSwap(F, X, i, k);
            }
        }

        public static void SecondRound(Digest MDq, uint blockSize, uint[] X, ref uint F, ref uint i, ref uint k)
        {
            for (; i < blockSize / 2; ++i)
            {
                k = (1 + (5 * i)) % (blockSize / 4);
                F = StepFunctions.FunctionG(MDq.B, MDq.C, MDq.D);

                MDq.IterationSwap(F, X, i, k);
            }
        }

        public static void ThirdRound(Digest MDq, uint blockSize, uint[] X, ref uint F, ref uint i, ref uint k)
        {
            for (; i < blockSize / 4 * 3; ++i)
            {
                k = (5 + (3 * i)) % (blockSize / 4);
                F = StepFunctions.FunctionH(MDq.B, MDq.C, MDq.D);

                MDq.IterationSwap(F, X, i, k);
            }
        }

        public static void FourthRound(Digest MDq, uint blockSize, uint[] X, ref uint F, ref uint i, ref uint k)
        {
            for (; i < blockSize; ++i)
            {
                k = 7 * i % (blockSize / 4);
                F = StepFunctions.FunctionI(MDq.B, MDq.C, MDq.D);

                MDq.IterationSwap(F, X, i, k);
            }
        }
    }
}

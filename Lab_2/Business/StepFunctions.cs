namespace Lab_2.Business
{
    internal class StepFunctions
    {
        public static uint FunctionF(uint B, uint C, uint D) => ((B & C) | (~B & D));
        public static uint FunctionG(uint B, uint C, uint D) => ((D & B) | (C & ~D));
        public static uint FunctionH(uint B, uint C, uint D) => (B ^ C ^ D);
        public static uint FunctionI(uint B, uint C, uint D) => (C ^ (B | ~D));
    }
}

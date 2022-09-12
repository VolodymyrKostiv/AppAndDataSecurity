namespace Lab_3.Interfaces
{
    internal interface IRC5Algorithm
    {
        byte[] EncipherCBCPAD(byte[] input, int numOfRounds, byte[] key);
        byte[] DecipherCBCPAD(byte[] input, int numOfRounds, byte[] key);
    }
}

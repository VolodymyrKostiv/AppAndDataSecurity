namespace Lab_3.Interfaces
{
    internal interface IRC5Algorithm
    {
        byte[] EncipherCBCPAD(string fileName, int numOfRounds, byte[] key);
        byte[] DecipherCBCPAD(string fileName, int numOfRounds, byte[] key);
    }
}

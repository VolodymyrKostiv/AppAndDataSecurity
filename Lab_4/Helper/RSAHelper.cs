using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lab_4.Helper
{
    internal static class RSAHelper
    {
        public static byte[] RSAEncrypt(byte[] MessageToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            byte[] encryptedMessage;

            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.ImportParameters(RSAKeyInfo);

                encryptedMessage = RSA.Encrypt(MessageToEncrypt, DoOAEPPadding);


            }
            return encryptedMessage;
        }

        public static byte[] RSADecrypt(byte[] MessageToDecrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            byte[] decryptedMessage;
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.ImportParameters(RSAKeyInfo);
                decryptedMessage = RSA.Decrypt(MessageToDecrypt, DoOAEPPadding);
            }
            return decryptedMessage;
        }
    }
}

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Lab_5.Models
{
    internal class DSS
    {
        private readonly DSACryptoServiceProvider _dsaService;
        private readonly SHA1 _sha1Service;

        public DSS()
        {
            _dsaService = new DSACryptoServiceProvider();
            _sha1Service = SHA1.Create();
        }

        public string CreateSignature(string message)
        {
            return CreateSignature(Encoding.UTF8.GetBytes(message));
        }

        public string CreateSignature(byte[] message)
        {
            string result = null;

            try
            {
                byte[] hash = _sha1Service.ComputeHash(message);
                byte[] signature = _dsaService.CreateSignature(hash);

                var y = _dsaService.ToXmlString(true);
                result = Convert.ToBase64String(signature);
            }
            catch (Exception ex)
            {
                result = string.Empty;
                throw new Exception(ex.Message);
            }

            return result;
        }

        public string GetParameters(bool includePrivate = true)
        {
            return _dsaService.ToXmlString(includePrivate);
        }

        public void ImportParameters(string parameters)
        {
            _dsaService.FromXmlString(parameters);
        }

        public bool VerifySignature(string message, string signature)
        {
            return VerifySignature(Encoding.UTF8.GetBytes(message), Convert.FromBase64String(signature));
        }

        public bool VerifySignature(byte[] message, byte[] signature)
        {
            bool result = false;    

            try
            {
                byte[] hash = _sha1Service.ComputeHash(message);
                result = _dsaService.VerifySignature(hash, signature);
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception(ex.Message);
            }

            return result;
        }
    }
}

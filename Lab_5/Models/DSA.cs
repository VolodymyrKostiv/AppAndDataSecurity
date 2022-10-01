using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lab_5.Models
{
    internal class DSA
    {
        private readonly DSACryptoServiceProvider _dsaService;
        private readonly SHA1 _sha1Service;

        public DSA()
        {
            _dsaService = new DSACryptoServiceProvider();
            _sha1Service = SHA1.Create();
        }

        public string CreateSignature(byte[] message)
        {
            byte[] hash = _sha1Service.ComputeHash(message);
            byte[] signature = _dsaService.CreateSignature(hash);
            string result = Convert.ToBase64String(signature);
            
            return result;
        }
    }
}

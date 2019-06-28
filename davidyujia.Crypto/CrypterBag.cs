using System;
using System.Security.Cryptography;
using System.Text;

namespace davidyujia.Crypto
{
    internal sealed class CrypterBag
    {
        public byte[] Key { get; private set; }
        public byte[] Vector { get; private set; }
        public CrypterBag(string key, string vector)
        {
            var md5 = new MD5CryptoServiceProvider();
            var sha256 = new SHA256CryptoServiceProvider();
            Key = sha256.ComputeHash(Encoding.ASCII.GetBytes(key));
            Vector = md5.ComputeHash(Encoding.ASCII.GetBytes(vector));
        }
    }
}
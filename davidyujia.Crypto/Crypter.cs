using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace davidyujia.Crypto
{
    public sealed class RSAKey
    {
        internal RSAKey()
        {
            var rsa = Crypter.GetRsaAlgorithm();

            PublicKey = rsa.ToXmlString(false);
            PrivateKey = rsa.ToXmlString(true);
        }

        public string PublicKey { get; }
        public string PrivateKey { get; }
    }

    public sealed class Crypter
    {
        private CrypterBag CrypterBag { get; }

        public Crypter()
        {
            CrypterBag = new CrypterBag(Environment.MachineName, Environment.MachineName);
        }

        public Crypter(string key, string vector)
        {
            CrypterBag = new CrypterBag(key, vector);
        }

        private SymmetricAlgorithm CreateAlgorithm()
        {
            SymmetricAlgorithm algorithm = new AesCryptoServiceProvider();
            algorithm.Key = CrypterBag.Key;
            algorithm.IV = CrypterBag.Vector;
            return algorithm;
        }

        public string Encrypt(string source)
        {
            var des = CreateAlgorithm();
            var data = Encoding.UTF8.GetBytes(source);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                    return Base58.Encode(ms.ToArray());
                }
            }
        }

        public string Decrypt(string encrypt)
        {
            var des = CreateAlgorithm();
            var data = Base58.Decode(encrypt);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }

        internal static RSACryptoServiceProvider GetRsaAlgorithm()
        {
            return new RSACryptoServiceProvider();
        }

        public static RSAKey GetRsaKey()
        {
            return new RSAKey();
        }

        public static string Encrypt(string source, string publicKey)
        {
            var rsa = GetRsaAlgorithm();
            rsa.FromXmlString(publicKey);
            var encryptData = rsa.Encrypt(Encoding.UTF8.GetBytes(source), false);
            return Base58.Encode(encryptData);
        }

        public static string Decrypt(string encrypt, string privateKey)
        {
            var rsa = GetRsaAlgorithm();
            rsa.FromXmlString(privateKey);
            var encryptData = Base58.Decode(encrypt);
            var decryptData = rsa.Decrypt(encryptData, false);
            var decryptString = Encoding.UTF8.GetString(decryptData);
            return decryptString;
        }
    }
}

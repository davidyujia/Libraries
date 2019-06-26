using System;
using System.IO;
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
            Key = Encoding.ASCII.GetBytes(key);
            Vector = Encoding.ASCII.GetBytes(vector);
        }
    }

    internal sealed class MachineCode
    {
        public MachineCode()
        {
            CrypterBag = new CrypterBag("123", "321");
        }

        public CrypterBag CrypterBag { get; }
    }

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

        private Lazy<MachineCode> Machine = new Lazy<MachineCode>(() => new MachineCode());

        private CrypterBag CrypterBag { get; }

        public Crypter()
        {
        }

        public Crypter(string key, string vector)
        {
            CrypterBag = new CrypterBag(key, vector);
        }

        private byte[] GetKey()
        {
            return CrypterBag == null ? Machine.Value.CrypterBag.Key : CrypterBag.Key;
        }

        private byte[] GetVector()
        {
            return CrypterBag == null ? Machine.Value.CrypterBag.Vector : CrypterBag.Vector;
        }

        private SymmetricAlgorithm CreateAlgorithm()
        {
            SymmetricAlgorithm algorithm = new DESCryptoServiceProvider();
            algorithm.Key = GetKey();
            algorithm.IV = GetVector();
            return algorithm;
        }

        public string Encode(string source)
        {
            var des = CreateAlgorithm();
            var data = Encoding.UTF8.GetBytes(source);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public string Decode(string encrypt)
        {
            var des = CreateAlgorithm();
            var data = Convert.FromBase64String(encrypt);
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

        public static string Encode(string source, string publicKey)
        {
            var rsa = GetRsaAlgorithm();
            rsa.FromXmlString(publicKey);
            var encryptData = rsa.Encrypt(Encoding.UTF8.GetBytes(source), false);
            return Convert.ToBase64String(encryptData);
        }

        public static string Decode(string encrypt, string privateKey)
        {
            var rsa = GetRsaAlgorithm();
            rsa.FromXmlString(privateKey);
            var encryptData = Convert.FromBase64String(encrypt);
            var decryptData = rsa.Decrypt(encryptData, false);
            var decryptString = Encoding.UTF8.GetString(decryptData);
            return decryptString;
        }

    }
}

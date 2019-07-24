using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace davidyujia.Crypto
{
    public abstract class BaseCode
    {
        private Lazy<char[]> codeTable;

        protected BaseCode()
        {
            codeTable = new Lazy<char[]>(GetCodeTable);
        }

        protected abstract char[] GetCodeTable();

        private char[] EncodeToCharArray(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            if (!data.Any())
            {
                return new char[0];
            }

            var sum = data.Aggregate<byte, BigInteger>(0, (current, b) => current * 256 + b);

            var sb = new StringBuilder();
            do
            {
                sum = BigInteger.DivRem(sum, codeTable.Value.Length, out var remainder);
                sb.Append(codeTable.Value[(int)remainder]);
            } while (sum != 0);

            return sb.ToString().Reverse().ToArray();
        }

        protected string EncodeCore(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            if (!data.Any())
            {
                return string.Empty;
            }

            var sum = data.Aggregate<byte, BigInteger>(0, (current, b) => current * 256 + b);

            var sb = new StringBuilder();
            do
            {
                sum = BigInteger.DivRem(sum, codeTable.Value.Length, out var remainder);
                sb.Append(codeTable.Value[(int)remainder]);
            } while (sum != 0);

            return new string(EncodeToCharArray(data));
        }

        protected byte[] DecodeCore(string encryptedString)
        {
            if (encryptedString == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(encryptedString))
            {
                return new byte[0];
            }
            var sum = encryptedString.ToArray().Aggregate<char, BigInteger>(0, (current, c) => (current * codeTable.Value.Length) + Array.IndexOf(codeTable.Value, c));

            var bit8 = 0;
            var list = new List<byte>();
            do
            {
                var twoPower = BigInteger.Pow(2, 8 * bit8++);
                var remainder = sum % (twoPower * 256);
                var quotient = remainder / twoPower;
                sum -= remainder;
                list.Add((byte)quotient);
            } while (sum != 0);
            list.Reverse();
            return list.ToArray();
        }
    }
}
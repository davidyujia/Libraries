using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using davidyujia.Configuration;
using davidyujia.Crypto;
using System.Text;

namespace davidyujia.Configuration.Crypter
{
    class Program
    {
        static void Main(string[] args)
        {
            // if (args == null || !args.Any())
            // {
            //     return;
            // }

            // if (!File.Exists(args[0]))
            // {
            //     return;
            // }
            //var jsonString = File.ReadAllText(args[0]);
            var jsonString = File.ReadAllText("test.json");
            dynamic jObj = JObject.Parse(jsonString);

            var s = jObj;
            foreach (var arg in args)
            {
                s = s[arg];
            }

            var st = s.ToString(Formatting.None);

            s = CrypterConfig.Encrypt(st);
            var x = CrypterConfig.Decrypt(s);

            var sw = CrypterConfig.Encrypt("PORT=5432;TIMEOUT=15;POOLING=True;MINPOOLSIZE=1;MAXPOOLSIZE=20;COMMANDTIMEOUT=20;HOST=10.6.200.122;DATABASE=IotRoadEventNetwork;USER ID=postgres;PASSWORD=K!ngw@ytek;SearchPath=public;");
            var ws = CrypterConfig.Decrypt(sw);

            var sxx = Base58.Encode(Encoding.UTF8.GetBytes("1234"));

        }
    }
}

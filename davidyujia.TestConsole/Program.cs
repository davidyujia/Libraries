using System;
using System.Threading.Tasks;
using davidyujia.Process;
using davidyujia.Configuration;
using davidyujia.Crypto;
using System.Text;
using System.Diagnostics;

namespace davidyujia.TestConsole
{
    class Program
    {
        private static QueueProcess<int> _process;

        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            var xsss = Encoding.UTF8.GetBytes("12");

            while (true)
            {
                sw.Reset();
                sw.Start();
                var x = Base58.Encode(xsss);
                sw.Stop();
                Console.WriteLine(xsss.Length);
                Console.WriteLine("E: " + sw.Elapsed.TotalSeconds);
                sw.Reset();
                sw.Start();
                var s = Base58.Decode(x);
                sw.Stop();
                Console.WriteLine("D: " + sw.Elapsed.TotalSeconds);
                Console.WriteLine(Encoding.UTF8.GetString(s));
                break;
            }
            Console.WriteLine("End");
        }


        static void ConfigTest()
        {
            Config.GetEnvironmentFunc = () => "Debug";
            Config.Environment = "";
            var c = Config.Load();

            var x = c["Run"]["Key"];

            Console.WriteLine(x);
        }

        static void ProcessTest()
        {
            var count = 0;
            var random = new Random(Guid.NewGuid().GetHashCode()).Next(100000, 100000);

            _process = new QueueProcess<int>(item =>
            {
                count++;
            });

            Parallel.For(0, random, (i, loopState) =>
            {
                _process.Add(i);
            });

            _process.Wait();
            Console.WriteLine(random);
            Console.WriteLine(count);
        }
    }
}

using System;
using System.Threading.Tasks;
using davidyujia.Process;
using davidyujia.Configuration;

namespace davidyujia.TestConsole
{
    class Program
    {
        private static QueueProcess<int> _process;

        static void Main(string[] args)
        {
            Config.GetEnvironmentFunc = () => "Debug";
            Config.Environment = "";
            var c = Config.Load();

            var x = c["Run"]["Key"];

            Console.WriteLine(x);

            Console.WriteLine("End");
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

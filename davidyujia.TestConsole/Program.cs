using System;
using System.Threading.Tasks;
using davidyujia.Process;

namespace davidyujia.TestConsole
{
    class Program
    {
        private static QueueProcess<int> _process;

        static void Main(string[] args)
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
            Console.WriteLine("End");
        }
    }
}

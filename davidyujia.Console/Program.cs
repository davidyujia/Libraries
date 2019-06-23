using System;
using System.Threading.Tasks;
using davidyujia.Process;

namespace davidyujia.Console
{
    class Program
    {private static QueueProcess<int> _process;

    static void Main(string[] args)
    {
        _process = new QueueProcess<int>(item => {
            Console.WriteLine(item);
        });

        Parallel.For(0, 100, (i, loopState) =>
        {
            _process.Add(i);
        });

        Console.Read();
    }
    }
}

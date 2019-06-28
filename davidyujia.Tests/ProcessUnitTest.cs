using davidyujia.Process;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace davidyujia.Tests
{
    [TestClass]
    public class ProcessUnitTest
    {
        private static QueueProcess<int> _process;

        [TestMethod]
        public void ProcessFeatureTest()
        {
            var random = new Random(Guid.NewGuid().GetHashCode()).Next(10000, 10000);

            var count = 0;
            _process = new QueueProcess<int>(item =>
            {
                count++;
            });

            Parallel.For(0, random, (i, loopState) =>
            {
                _process.Add(i);
            });

            _process.Wait();

            Assert.AreEqual(random, count);
        }
    }
}

using System.Collections.Concurrent;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Radoslav.Common.UnitTests
{
    [TestClass]
    public sealed class ConcurrentCollectionsTests
    {
        [TestMethod]
        public void AddDuringEnumerateDictionary()
        {
            ConcurrentDictionary<int, object> dictionary = new ConcurrentDictionary<int, object>();

            Assert.IsTrue(dictionary.TryAdd(1, 1));

            ThreadPool.QueueUserWorkItem(state =>
            {
                Thread.Sleep(500);

                dictionary.TryAdd(2, 2);
            });

            dictionary.ForEach(false, i => Thread.Sleep(1000));

            Assert.AreEqual(2, dictionary.Count);
        }

        [TestMethod]
        public void AddDuringEnumerateDictionaryValues()
        {
            ConcurrentDictionary<int, object> dictionary = new ConcurrentDictionary<int, object>();

            Assert.IsTrue(dictionary.TryAdd(1, 1));

            ThreadPool.QueueUserWorkItem(state =>
            {
                Thread.Sleep(500);

                dictionary.TryAdd(2, 2);
            });

            dictionary.Values.ForEach(false, i => Thread.Sleep(1000));

            Assert.AreEqual(2, dictionary.Count);
        }

        [TestMethod]
        public void AddDuringEnumerateBag()
        {
            ConcurrentBag<int> bag = new ConcurrentBag<int>();

            bag.Add(1);

            ThreadPool.QueueUserWorkItem(state =>
            {
                Thread.Sleep(500);

                bag.Add(2);
            });

            bag.ForEach(false, i => Thread.Sleep(1000));

            Assert.AreEqual(2, bag.Count);
        }
    }
}
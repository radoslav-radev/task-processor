using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Collections;

namespace Radoslav.Common.UnitTests
{
    [TestClass]
    public sealed class ConcurrentHashSetTests
    {
        #region Properties

        public TestContext TestContext { get; set; }

        private ConcurrentHashSet<object> ConcurrentHashSet
        {
            get
            {
                return (ConcurrentHashSet<object>)this.TestContext.Properties[typeof(ConcurrentHashSet<>).Name];
            }

            set
            {
                this.TestContext.Properties.Add(typeof(ConcurrentHashSet<>).Name, value);
            }
        }

        private ReaderWriterLockSlim Locker
        {
            get
            {
                return (ReaderWriterLockSlim)this.TestContext.Properties[typeof(ReaderWriterLockSlim).Name];
            }

            set
            {
                this.TestContext.Properties.Add(typeof(ReaderWriterLockSlim).Name, value);
            }
        }

        #endregion Properties

        #region Initializes & Cleanup

        [TestInitialize]
        public void InitializeTest()
        {
            this.ConcurrentHashSet = new ConcurrentHashSet<object>();

            this.Locker = (ReaderWriterLockSlim)this.ConcurrentHashSet.GetType().BaseType
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .Single(f => f.FieldType == typeof(ReaderWriterLockSlim))
                .GetValue(this.ConcurrentHashSet);
        }

        #endregion Initializes & Cleanup

        [TestMethod]
        public void AddDuringEnumerate()
        {
            this.ConcurrentHashSet.Add(1);

            ThreadPool.QueueUserWorkItem(state =>
            {
                Thread.Sleep(500);

                this.ConcurrentHashSet.Add(2);
            });

            this.ConcurrentHashSet.ForEach(false, i => Thread.Sleep(1000));

            Assert.AreEqual(2, this.ConcurrentHashSet.Count);
        }

        [TestMethod]
        public void RemoveDuringEnumerate()
        {
            this.ConcurrentHashSet.Add(1);
            this.ConcurrentHashSet.Add(2);

            ThreadPool.QueueUserWorkItem(state =>
            {
                Thread.Sleep(500);

                this.ConcurrentHashSet.Remove(1);
            });

            this.ConcurrentHashSet.ForEach(false, i => Thread.Sleep(1000));
        }

        [TestMethod]
        public void ReleaseLockAfterEnumerate()
        {
            this.ConcurrentHashSet.Add(1);

            this.ConcurrentHashSet.ForEach(false, item => { });

            Assert.IsFalse(this.Locker.IsReadLockHeld);
        }

        [TestMethod]
        public void AcquireLockDuringEnumerate()
        {
            this.ConcurrentHashSet.Add(1);

            this.ConcurrentHashSet.ForEach(false, item => Assert.IsTrue(this.Locker.IsReadLockHeld));

            Assert.IsFalse(this.Locker.IsReadLockHeld);
        }

        [TestMethod]
        public void ReleaseLockOnEnumeratorDispose()
        {
            this.ConcurrentHashSet.Add(1);

            IEnumerator enumerator = this.ConcurrentHashSet.GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsTrue(this.Locker.IsReadLockHeld);

            ((IDisposable)enumerator).Dispose();

            Assert.IsFalse(this.Locker.IsReadLockHeld);
        }

        [TestMethod]
        public void ReleaseLockOnEnumeratorReset()
        {
            this.ConcurrentHashSet.Add(1);

            IEnumerator enumerator = this.ConcurrentHashSet.GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsTrue(this.Locker.IsReadLockHeld);

            enumerator.Reset();

            Assert.IsFalse(this.Locker.IsReadLockHeld);
        }

        [TestMethod]
        public void ParallelForEach()
        {
            this.ConcurrentHashSet.AddRange(1, 2, 3, 4);

            Parallel.ForEach(this.ConcurrentHashSet, item => { });

            Assert.IsFalse(this.Locker.IsReadLockHeld);
            Assert.AreEqual(0, this.Locker.CurrentReadCount);
        }

        [TestMethod]
        public void ExecuteWithReadLock()
        {
            this.ConcurrentHashSet.ExecuteWithReadLock(() =>
                {
                    Assert.IsTrue(this.Locker.IsReadLockHeld);
                });

            Assert.IsFalse(this.Locker.IsReadLockHeld);
        }

        [TestMethod]
        public void ExecuteReadOperationInReadLock()
        {
            this.ConcurrentHashSet.ExecuteWithReadLock(() =>
            {
                this.ConcurrentHashSet.Contains(1);
            });

            Assert.IsFalse(this.Locker.IsReadLockHeld);
        }

        [TestMethod]
        [ExpectedException(typeof(LockRecursionException))]
        public void ExecuteWriteOperationInReadLock()
        {
            this.ConcurrentHashSet.ExecuteWithReadLock(() =>
            {
                this.ConcurrentHashSet.Add(1);
            });
        }

        [TestMethod]
        public void ExecuteWithWriteLock()
        {
            this.ConcurrentHashSet.ExecuteWithWriteLock(() =>
            {
                Assert.IsTrue(this.Locker.IsWriteLockHeld);
            });

            Assert.IsFalse(this.Locker.IsWriteLockHeld);
        }

        [TestMethod]
        public void ExecuteReadOperationInWriteLock()
        {
            this.ConcurrentHashSet.ExecuteWithWriteLock(() =>
            {
                this.ConcurrentHashSet.Contains(1);
            });

            Assert.IsFalse(this.Locker.IsReadLockHeld);
            Assert.IsFalse(this.Locker.IsWriteLockHeld);
        }

        [TestMethod]
        public void ExecuteWriteOperationInWriteLock()
        {
            this.ConcurrentHashSet.ExecuteWithWriteLock(() =>
            {
                this.ConcurrentHashSet.Add(1);
            });

            Assert.IsFalse(this.Locker.IsWriteLockHeld);
        }
    }
}
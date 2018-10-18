using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Redis.ServiceStack;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    [SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors", Justification = "This class is needed because of Assembly Initialize and Assembly Cleanup methods.")]
    public sealed class AssemblyUnitTests
    {
        private static readonly object RedisLock = new object();

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            AssemblyUnitTests.ClearRedis();
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            AssemblyUnitTests.ClearRedis();
        }

        internal static void EnterRedisLock()
        {
            Monitor.Enter(AssemblyUnitTests.RedisLock);

            AssemblyUnitTests.ClearRedis();
        }

        internal static void ExitRedisLock()
        {
            AssemblyUnitTests.ClearRedis();

            Monitor.Exit(AssemblyUnitTests.RedisLock);
        }

        private static void ClearRedis()
        {
            using (ServiceStackRedisProvider provider = new ServiceStackRedisProvider())
            {
                provider.FlushAll();
            }
        }
    }
}
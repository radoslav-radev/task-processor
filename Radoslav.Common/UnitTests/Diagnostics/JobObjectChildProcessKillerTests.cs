using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Diagnostics;

namespace Radoslav.Common.UnitTests
{
    [TestClass]
    public sealed class JobObjectChildProcessKillerTests : ChildProcessKillerTestsBase
    {
        protected override IChildProcessKiller CreateChildProcessKiller()
        {
            return new JobObjectChildProcessKiller();
        }
    }
}
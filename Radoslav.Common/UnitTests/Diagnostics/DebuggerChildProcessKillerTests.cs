using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Diagnostics;

namespace Radoslav.Common.UnitTests.Diagnostics
{
    [TestClass]
    public sealed class DebuggerChildProcessKillerTests : ChildProcessKillerTestsBase
    {
        protected override IChildProcessKiller CreateChildProcessKiller()
        {
            return new DebuggerChildProcessKiller();
        }
    }
}
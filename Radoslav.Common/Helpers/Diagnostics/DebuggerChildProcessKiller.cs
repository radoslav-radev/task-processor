using System;
using System.Diagnostics;
using System.Threading;

namespace Radoslav.Diagnostics
{
    /// <summary>
    /// An implementation of <see cref="IChildProcessKiller"/> that attaches a debugger from the parent process to the child one.
    /// </summary>
    /// <remarks>More details at: <a href="http://stackoverflow.com/a/24012744">Stack Overflow</a>.</remarks>
    public sealed class DebuggerChildProcessKiller : IChildProcessKiller
    {
        #region IChildProcessKiller Members

        /// <inheritdoc />
        public void AddProcess(Process process)
        {
            if (process == null)
            {
                throw new ArgumentNullException("process");
            }

            Thread thread = new Thread(DebuggerChildProcessKiller.NullDebugger)
            {
                IsBackground = true
            };

            thread.Start(process);
        }

        #endregion IChildProcessKiller Members

        private static void NullDebugger(object arg)
        {
            Process childProcess = (Process)arg;

            // Attach to the process we provided the thread as an argument
            if (NativeMethods.DebugActiveProcess(childProcess.Id))
            {
                var debugEvent = new NativeMethods.DEBUG_EVENT
                {
                    bytes = new byte[1024]
                };

                while (!childProcess.HasExited)
                {
                    if (NativeMethods.WaitForDebugEvent(out debugEvent, 1000))
                    {
                        // return DBG_CONTINUE for all events but the exception type
                        var continueFlag = NativeMethods.DBG_CONTINUE;

                        if (debugEvent.dwDebugEventCode == NativeMethods.DebugEventType.EXCEPTION_DEBUG_EVENT)
                        {
                            continueFlag = NativeMethods.DBG_EXCEPTION_NOT_HANDLED;
                        }

                        NativeMethods.ContinueDebugEvent(debugEvent.dwProcessId, debugEvent.dwThreadId, continueFlag);
                    }
                }
            }
            else
            {
                Trace.TraceWarning("Unable to attach debugger");
            }
        }
    }
}
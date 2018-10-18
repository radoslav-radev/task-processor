using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Radoslav.Diagnostics
{
    /// <summary>
    /// An implementation of <see cref="IChildProcessKiller"/> that uses Windows Job Objects.
    /// </summary>
    /// <remarks>More details at: <a href="http://stackoverflow.com/a/4657392">Stack Overflow</a>.</remarks>
    public sealed class JobObjectChildProcessKiller : IChildProcessKiller, IDisposable
    {
        [SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources", Justification = SuppressMessages.CopyPasteCode)]
        private IntPtr jobHandle;

        private bool isDsposed;

        #region Constructor & Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="JobObjectChildProcessKiller"/> class.
        /// </summary>
        public JobObjectChildProcessKiller()
        {
            this.jobHandle = NativeMethods.CreateJobObject(null, null);

            NativeMethods.JOBOBJECT_BASIC_LIMIT_INFORMATION info = new NativeMethods.JOBOBJECT_BASIC_LIMIT_INFORMATION()
            {
                LimitFlags = 0x2000
            };

            NativeMethods.JOBOBJECT_EXTENDED_LIMIT_INFORMATION extendedInfo = new NativeMethods.JOBOBJECT_EXTENDED_LIMIT_INFORMATION()
            {
                BasicLimitInformation = info
            };

            int length = Marshal.SizeOf(typeof(NativeMethods.JOBOBJECT_EXTENDED_LIMIT_INFORMATION));

            IntPtr extendedInfoPtr = Marshal.AllocHGlobal(length);

            Marshal.StructureToPtr(extendedInfo, extendedInfoPtr, false);

            if (!NativeMethods.SetInformationJobObject(this.jobHandle, NativeMethods.JobObjectInfoType.ExtendedLimitInformation, extendedInfoPtr, (uint)length))
            {
                throw new NativeException("Unable to set information. Error: " + Marshal.GetLastWin32Error());
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="JobObjectChildProcessKiller"/> class.
        /// </summary>
        ~JobObjectChildProcessKiller()
        {
            this.Dispose();
        }

        #endregion Constructor & Destructor

        #region IDisposable Members

        /// <inheritdoc />
        public void Dispose()
        {
            if (this.isDsposed)
            {
                return;
            }

            NativeMethods.CloseHandle(this.jobHandle);

            this.jobHandle = IntPtr.Zero;

            GC.SuppressFinalize(this);

            this.isDsposed = true;
        }

        #endregion IDisposable Members

        #region IChildProcessKiller

        /// <inheritdoc />
        public void AddProcess(Process process)
        {
            if (process == null)
            {
                throw new ArgumentNullException("process");
            }

            if (!NativeMethods.AssignProcessToJobObject(this.jobHandle, process.Handle))
            {
                throw new NativeException("Failed to assign process to job object.");
            }
        }

        #endregion IChildProcessKiller
    }
}
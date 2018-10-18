using System;
using System.Runtime.InteropServices;

namespace Radoslav.Diagnostics
{
    internal static class NativeMethods
    {
        internal const int DBG_CONTINUE = 0x00010002;
        internal const int DBG_EXCEPTION_NOT_HANDLED = unchecked((int)0x80010001);

        internal enum JobObjectInfoType
        {
            AssociateCompletionPortInformation = 7,
            BasicLimitInformation = 2,
            BasicUIRestrictions = 4,
            EndOfJobTimeInformation = 6,
            ExtendedLimitInformation = 9,
            SecurityLimitInformation = 5,
            GroupInformation = 11
        }

        internal enum DebugEventType : int
        {
            CREATE_PROCESS_DEBUG_EVENT = 3,

            // Reports a create-process debugging event. The value of u.CreateProcessInfo specifies a CREATE_PROCESS_DEBUG_INFO structure.
            CREATE_THREAD_DEBUG_EVENT = 2,

            // Reports a create-thread debugging event. The value of u.CreateThread specifies a CREATE_THREAD_DEBUG_INFO structure.
            EXCEPTION_DEBUG_EVENT = 1,

            // Reports an exception debugging event. The value of u.Exception specifies an EXCEPTION_DEBUG_INFO structure.
            EXIT_PROCESS_DEBUG_EVENT = 5,

            // Reports an exit-process debugging event. The value of u.ExitProcess specifies an EXIT_PROCESS_DEBUG_INFO structure.
            EXIT_THREAD_DEBUG_EVENT = 4,

            // Reports an exit-thread debugging event. The value of u.ExitThread specifies an EXIT_THREAD_DEBUG_INFO structure.
            LOAD_DLL_DEBUG_EVENT = 6,

            // Reports a load-dynamic-link-library (DLL) debugging event. The value of u.LoadDll specifies a LOAD_DLL_DEBUG_INFO structure.
            OUTPUT_DEBUG_STRING_EVENT = 8,

            // Reports an output-debugging-string debugging event. The value of u.DebugString specifies an OUTPUT_DEBUG_STRING_INFO structure.
            RIP_EVENT = 9,

            // Reports a RIP-debugging event (system debugging error). The value of u.RipInfo specifies a RIP_INFO structure.
            UNLOAD_DLL_DEBUG_EVENT = 7,

            // Reports an unload-DLL debugging event. The value of u.UnloadDll specifies an UNLOAD_DLL_DEBUG_INFO structure.
        }

        [DllImport("Kernel32")]
        internal static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateJobObject(object a, string lpName);

        [DllImport("kernel32.dll")]
        internal static extern bool SetInformationJobObject(IntPtr hJob, JobObjectInfoType infoType, IntPtr lpJobObjectInfo, uint cbJobObjectInfoLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);

        [DllImport("Kernel32.dll", SetLastError = true)]
        internal static extern bool DebugActiveProcess(int dwProcessId);

        [DllImport("Kernel32.dll", SetLastError = true)]
        internal static extern bool WaitForDebugEvent([Out] out DEBUG_EVENT lpDebugEvent, int dwMilliseconds);

        [DllImport("Kernel32.dll", SetLastError = true)]
        internal static extern bool ContinueDebugEvent(int dwProcessId, int dwThreadId, int dwContinueStatus);

        [StructLayout(LayoutKind.Sequential)]
        internal struct DEBUG_EVENT
        {
            [MarshalAs(UnmanagedType.I4)]
            public DebugEventType dwDebugEventCode;

            public int dwProcessId;
            public int dwThreadId;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public byte[] bytes;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
        {
            public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
            public IO_COUNTERS IoInfo;
            public uint ProcessMemoryLimit;
            public uint JobMemoryLimit;
            public uint PeakProcessMemoryUsed;
            public uint PeakJobMemoryUsed;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct IO_COUNTERS
        {
            public ulong ReadOperationCount;
            public ulong WriteOperationCount;
            public ulong OtherOperationCount;
            public ulong ReadTransferCount;
            public ulong WriteTransferCount;
            public ulong OtherTransferCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct JOBOBJECT_BASIC_LIMIT_INFORMATION
        {
            public long PerProcessUserTimeLimit;
            public long PerJobUserTimeLimit;
            public short LimitFlags;
            public uint MinimumWorkingSetSize;
            public uint MaximumWorkingSetSize;
            public short ActiveProcessLimit;
            public long Affinity;
            public short PriorityClass;
            public short SchedulingClass;
        }
    }
}
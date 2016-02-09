using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace ConsoleApplication.Privileges
{
    public sealed class AccessTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal AccessTokenHandle(ProcessHandle processHandle, TokenAccessRights tokenAccessRights)
            : base(true)
        {
            if (!NativeMethods.OpenProcessToken(processHandle, tokenAccessRights, ref handle))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            if (!NativeMethods.CloseHandle(handle))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return true;
        }
    }
}

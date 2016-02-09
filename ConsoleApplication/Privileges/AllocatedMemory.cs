using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ConsoleApplication.Privileges
{
    internal sealed class AllocatedMemory : IDisposable
    {
        [SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources", Justification = "Not pointing to a native resource.")]
        private IntPtr _pointer;

        internal AllocatedMemory(int bytesRequired)
        {
            _pointer = Marshal.AllocHGlobal(bytesRequired);
        }

        ~AllocatedMemory()
        {
            InternalDispose();
        }

        internal IntPtr Pointer
        {
            get
            {
                return _pointer;
            }
        }

        public void Dispose()
        {
            InternalDispose();
            GC.SuppressFinalize(this);
        }

        private void InternalDispose()
        {
            if (_pointer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_pointer);
                _pointer = IntPtr.Zero;
            }
        }
    }
}

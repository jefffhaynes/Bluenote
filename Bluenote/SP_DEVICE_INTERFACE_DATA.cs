using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Bluenote
{
    [StructLayout(LayoutKind.Sequential)]
    internal class SP_DEVICE_INTERFACE_DATA
    {
        internal uint cbSize = (uint)Marshal.SizeOf(typeof(SP_DEVICE_INTERFACE_DATA));
        internal Guid interfaceClassGuid = Guid.Empty;
        internal int flags = 0;
        internal int reserved = 0;
    }
}

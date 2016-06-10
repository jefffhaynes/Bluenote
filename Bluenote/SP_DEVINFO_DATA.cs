using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Bluenote
{
    [StructLayout(LayoutKind.Sequential)]
    internal class SP_DEVINFO_DATA
    {
        internal uint cbSize = (uint)Marshal.SizeOf(typeof(SP_DEVINFO_DATA));
        internal Guid classGuid = Guid.Empty;
        internal int devInst = 0;
        internal int reserved = 0;
    }
}

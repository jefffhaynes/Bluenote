using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace BluetoothGATTInterop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SP_DEVINFO_DATA
    {
        public uint cbSize;
        public Guid classGuid;
        public uint devInst;
        public IntPtr reserved;
    }
}

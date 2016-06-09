using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace BluetoothGATTInterop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SP_DEVICE_INTERFACE_DATA
    {
        public Int32 cbSize;
        public Guid interfaceClassGuid;
        public Int32 flags;
        private UIntPtr reserved;
    }
}

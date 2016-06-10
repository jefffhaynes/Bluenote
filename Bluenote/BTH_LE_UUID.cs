using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Bluenote
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BTH_LE_UUID
    {
        [MarshalAs(UnmanagedType.U1)]
        internal bool isShortUuid;

        internal ushort shortUuid;
        internal Guid longUuid;
    }
}

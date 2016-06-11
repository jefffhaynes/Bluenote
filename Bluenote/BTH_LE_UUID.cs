using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Bluenote
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BTH_LE_UUID
    {
        //[FieldOffset(0)]
        internal bool isShortUuid;
        
        //[FieldOffset(4)]
        internal ushort shortUuid;

        //[FieldOffset(4)]
        internal Guid longUuid;
    }
}

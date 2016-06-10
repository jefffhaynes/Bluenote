using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Bluenote
{
    [StructLayout(LayoutKind.Sequential)]
    internal class BTH_LE_UUID
    {
        internal bool isShortUuid;
        internal ushort shortUuid;
        internal Guid longUuid;
    }
}

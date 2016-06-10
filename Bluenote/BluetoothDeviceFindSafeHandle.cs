using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace Bluenote
{
    //[SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    //[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    //public class BluetoothDeviceFindSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    //{
    //    public BluetoothDeviceFindSafeHandle() : base(true)
    //    {
    //    }

    //    protected override bool ReleaseHandle()
    //    {
    //        return Interop.BluetoothFindDeviceClose(handle);
    //    }
    //}
}

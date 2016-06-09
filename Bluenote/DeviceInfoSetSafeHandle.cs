using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace BluetoothGATTInterop
{
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    public class DeviceInfoSetSafeHandle : SafeHandleMinusOneIsInvalid
    {
        public DeviceInfoSetSafeHandle() : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            return Interop.SetupDiDestroyDeviceInfoList(handle);
        }
    }
}

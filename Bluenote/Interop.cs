using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Bluenote
{
    internal static class Interop
    {
        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern DeviceInfoSetSafeHandle SetupDiGetClassDevs(
            ref Guid classGuid,
            IntPtr enumerator,
            IntPtr hwndParent,
            DiGetClassFlags flags
            );

        [DllImport("setupapi.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static extern bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInfo(DeviceInfoSetSafeHandle deviceInfoSet, uint memberIndex,
            ref SP_DEVINFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetupDiGetDeviceRegistryProperty(
            DeviceInfoSetSafeHandle deviceInfoSet,
            [In, Out] SP_DEVINFO_DATA deviceInfoData,
            SetupDiGetDeviceRegistryProperty property,
            out uint propertyRegDataType,
            byte[] propertyBuffer,
            uint propertyBufferSize,
            out uint requiredSize
            );

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SetupDiEnumDeviceInterfaces(
            DeviceInfoSetSafeHandle deviceInfoSet,
            SP_DEVINFO_DATA deviceInfoData,
            ref Guid interfaceClassGuid,
            uint memberIndex,
            [In, Out] SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(
            DeviceInfoSetSafeHandle hDevInfo,
            [In, Out] SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
            IntPtr deviceInterfaceDetailData,
            uint deviceInterfaceDetailDataSize,
            out uint requiredSize,
            [In, Out] SP_DEVINFO_DATA deviceInfoData);

        //[DllImport("irprops.cpl", SetLastError = true)]
        //public static extern BluetoothDeviceFindSafeHandle BluetoothFindFirstDevice(
        //    ref BLUETOOTH_DEVICE_SEARCH_PARAMS searchParams, ref BLUETOOTH_DEVICE_INFO deviceInfo);

        //[DllImport("Irprops.cpl", SetLastError = true)]
        //public static extern bool BluetoothFindNextDevice(BluetoothDeviceFindSafeHandle hFind, ref BLUETOOTH_DEVICE_INFO pbtdi);

        //[DllImport("irprops.cpl", SetLastError = true)]
        //public static extern bool BluetoothFindDeviceClose(IntPtr hFind);
    }
}

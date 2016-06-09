using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace BluetoothGATTInterop
{
    internal static class Interop
    {
        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern DeviceInfoSetSafeHandle SetupDiGetClassDevs(
            ref Guid classGuid,
            IntPtr enumerator,
            IntPtr hwndParent,
            DiGetClassFlags flags
            );

        [DllImport("setupapi.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static extern bool SetupDiDestroyDeviceInfoList
            (
            IntPtr deviceInfoSet
            );

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInfo(DeviceInfoSetSafeHandle deviceInfoSet, uint memberIndex,
            ref SP_DEVINFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiGetDeviceRegistryProperty(
            DeviceInfoSetSafeHandle deviceInfoSet,
            ref SP_DEVINFO_DATA deviceInfoData,
            SetupDiGetDeviceRegistryProperty property,
            out uint propertyRegDataType,
            byte[] propertyBuffer,
            uint propertyBufferSize,
            out uint requiredSize
            );

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInterfaces(
            DeviceInfoSetSafeHandle hDevInfo,
            ref SP_DEVINFO_DATA devInfo,
            ref Guid interfaceClassGuid,
            uint memberIndex,
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData
            );

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(
            DeviceInfoSetSafeHandle hDevInfo,
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
            IntPtr deviceInterfaceDetailData,
            uint deviceInterfaceDetailDataSize,
            out uint requiredSize,
            IntPtr deviceInfoData
            );
    }
}

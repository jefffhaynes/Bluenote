using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Bluenote
{
    internal static class Interop
    {
        public const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint CREATE_NEW = 1;
        public const uint CREATE_ALWAYS = 2;
        public const uint OPEN_EXISTING = 3;

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern DeviceInfoSetSafeHandle SetupDiGetClassDevs(
            ref Guid classGuid,
            IntPtr enumerator,
            IntPtr hwndParent,
            DiGetClassFlags flags);

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
            out uint requiredSize);

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

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess,
            uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition,
            uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("BluetoothAPIs.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int BluetoothGATTGetServices(
            SafeHandle handle,
            ushort serviceBufferCount,
            IntPtr serviceBuffer,
            out ushort serviceBufferActual,
            uint flags);
    }
}
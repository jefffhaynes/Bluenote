using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Bluenote
{
    public static class BluetoothDeviceManager
    {
        private static readonly Guid BluetoothServiceClassId = new Guid("e0cbf06c-cd8b-4647-bb8a-263b43f0f974");
        private static readonly Guid BluetoothInterfaceServiceClassId = new Guid("781aee18-7733-4ce4-add0-91f41c67b592");

        private const int ErrorInsufficientBuffer = 122;

        public static IEnumerable<string> GetDevices()
        {
            return GetDevices(BluetoothServiceClassId);
        }

        public static IEnumerable<string> GetDevices(Guid serviceClass)
        {
            using (DeviceInfoSetSafeHandle deviceInfoSet = SetupDiGetClassDevs(serviceClass, DiGetClassFlags.DIGCF_PRESENT))
            {
                SP_DEVINFO_DATA deviceInfoData = new SP_DEVINFO_DATA();

                for (uint i = 0; Interop.SetupDiEnumDeviceInfo(deviceInfoSet, i, ref deviceInfoData); i++)
                {
                    // loop on SetupDiGetDeviceRegistryProperty until we have enough space for returned property
                    uint propertyRegDataType;
                    byte[] propertyBuffer = new byte[0];
                    uint propertyBufferSize;
                    while (!Interop.SetupDiGetDeviceRegistryProperty(deviceInfoSet, deviceInfoData,
                        SetupDiGetDeviceRegistryProperty.SPDRP_DEVICEDESC, out propertyRegDataType, propertyBuffer,
                        (uint) propertyBuffer.Length, out propertyBufferSize))
                    {
                        if (Marshal.GetLastWin32Error() == ErrorInsufficientBuffer)
                        {
                            // Double the size to avoid problems on W2k MBCS systems per KB 888609. 
                            propertyBuffer = new byte[propertyBufferSize*2];
                        }
                        else
                        {
                            break;
                        }
                    }

                    yield return Encoding.Unicode.GetString(propertyBuffer);
                }
            }
        }
        
        public static IEnumerable<string> GetDeviceInterfaces()
        {
            return GetDeviceInterfaces(BluetoothInterfaceServiceClassId);
        }

        public static IEnumerable<string> GetDeviceInterfaces(Guid serviceClass)
        {
            using (DeviceInfoSetSafeHandle deviceInfoSet = SetupDiGetClassDevs(serviceClass, DiGetClassFlags.DIGCF_PRESENT | DiGetClassFlags.DIGCF_DEVICEINTERFACE))
            {
                SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();

                for (uint i = 0;
                    Interop.SetupDiEnumDeviceInterfaces(deviceInfoSet, null, ref serviceClass, i, deviceInterfaceData);
                    i++)
                {
                    uint requiredSize;
                    SP_DEVINFO_DATA deviceInfoData = new SP_DEVINFO_DATA();
                    if (!Interop.SetupDiGetDeviceInterfaceDetail(deviceInfoSet, deviceInterfaceData, IntPtr.Zero, 0,
                        out requiredSize, deviceInfoData))
                    {
                        IntPtr deviceInterfaceDetailData = Marshal.AllocHGlobal((int) requiredSize);

                        try
                        {
                            // check for 32 vs 64 bit system
                            var size = IntPtr.Size == 8 ? 8 : 4 + Marshal.SystemDefaultCharSize; 

                            Marshal.WriteInt32(deviceInterfaceDetailData, size);

                            if (!Interop.SetupDiGetDeviceInterfaceDetail(deviceInfoSet, deviceInterfaceData,
                                deviceInterfaceDetailData, requiredSize, out requiredSize, deviceInfoData))
                                throw new Win32Exception();

                            var file = Marshal.PtrToStringAuto(deviceInterfaceDetailData + 4);

                            var services = GetServices(file);

                            foreach (var service in services)
                            {
                                Console.WriteLine(service.serviceUuid.shortUuid);
                            }
                            yield return file;
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(deviceInterfaceDetailData);
                        }
                    }
                }
            }
        }

        private static DeviceInfoSetSafeHandle SetupDiGetClassDevs(Guid serviceClass, DiGetClassFlags classFlags)
        {
            return Interop.SetupDiGetClassDevs(ref serviceClass, IntPtr.Zero, IntPtr.Zero, classFlags);
        }

        private static IEnumerable<BTH_LE_GATT_SERVICE> GetServices(string serviceFile)
        {
            var fileHandle = Interop.CreateFile(serviceFile, Interop.GENERIC_WRITE | Interop.GENERIC_READ, 0,
                IntPtr.Zero, Interop.OPEN_EXISTING, Interop.FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);
            
            ushort serviceBufferCount;
            Interop.BluetoothGATTGetServices(fileHandle, 0, IntPtr.Zero, out serviceBufferCount, 0);

            var serviceSize = Marshal.SizeOf(typeof (BTH_LE_GATT_SERVICE));
            var serviceBufferLength = serviceSize * serviceBufferCount;
            IntPtr serviceBuffer = Marshal.AllocHGlobal(serviceBufferLength);

            try
            {
                Interop.BluetoothGATTGetServices(fileHandle, (ushort)serviceBufferLength, serviceBuffer, out serviceBufferCount, 0);
                for (int i = 0; i < serviceBufferCount; i++)
                {
                    var servicePtr = IntPtr.Add(serviceBuffer, serviceSize*i);
                    yield return (BTH_LE_GATT_SERVICE)Marshal.PtrToStructure(servicePtr, typeof (BTH_LE_GATT_SERVICE));
                }
            }
            finally
            {
                Marshal.FreeHGlobal(serviceBuffer);
            }
        }

        //public static void FindDevices()
        //{
        //    var searchParams = new BLUETOOTH_DEVICE_SEARCH_PARAMS
        //    {
        //        fReturnConnected = true,
        //        fIssueInquiry = true,
        //        cTimeoutMultiplier = 1
        //    };

        //    searchParams.Initialize();

        //    var deviceInfo = new BLUETOOTH_DEVICE_INFO();
        //    deviceInfo.Initialize();

        //    BluetoothDeviceFindSafeHandle handle;
        //    using (handle = Interop.BluetoothFindFirstDevice(ref searchParams, ref deviceInfo))
        //    {
        //        var error = Marshal.GetLastWin32Error();

        //        if (handle.IsInvalid)
        //            return;

        //        while (Interop.BluetoothFindNextDevice(handle, ref deviceInfo))
        //        {
                    
        //        }
        //    }
        //}
    }
}

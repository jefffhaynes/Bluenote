using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace Bluenote
{
    public static class BluetoothDeviceManager
    {
        private const int ErrorInsufficientBuffer = 122;
        private static readonly Guid BluetoothServiceClassId = new Guid("e0cbf06c-cd8b-4647-bb8a-263b43f0f974");
        private static readonly Guid BluetoothInterfaceServiceClassId = new Guid("781aee18-7733-4ce4-add0-91f41c67b592");
        
        public static IEnumerable<string> GetDevices()
        {
            return GetDevices(BluetoothServiceClassId);
        }

        public static IEnumerable<string> GetDevices(Guid serviceClass)
        {
            using (var deviceInfoSet = SetupDiGetClassDevs(serviceClass, DiGetClassFlags.DIGCF_PRESENT))
            {
                var deviceInfoData = new SP_DEVINFO_DATA();

                for (uint i = 0; Interop.SetupDiEnumDeviceInfo(deviceInfoSet, i, ref deviceInfoData); i++)
                {
                    // loop on SetupDiGetDeviceRegistryProperty until we have enough space for returned property
                    uint propertyRegDataType;
                    var propertyBuffer = new byte[0];
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

        public static IEnumerable<BluetoothService> GetDeviceInterfaces()
        {
            return GetDeviceInterfaces(BluetoothInterfaceServiceClassId);
        }

        public static IEnumerable<BluetoothService> GetDeviceInterfaces(Guid serviceClass)
        {
            using (
                var deviceInfoSet = SetupDiGetClassDevs(serviceClass,
                    DiGetClassFlags.DIGCF_PRESENT | DiGetClassFlags.DIGCF_DEVICEINTERFACE))
            {
                var deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();

                for (uint i = 0;
                    Interop.SetupDiEnumDeviceInterfaces(deviceInfoSet, null, ref serviceClass, i, deviceInterfaceData);
                    i++)
                {
                    uint requiredSize;
                    var deviceInfoData = new SP_DEVINFO_DATA();
                    if (!Interop.SetupDiGetDeviceInterfaceDetail(deviceInfoSet, deviceInterfaceData, IntPtr.Zero, 0,
                        out requiredSize, deviceInfoData))
                    {
                        var deviceInterfaceDetailData = Marshal.AllocHGlobal((int) requiredSize);

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
                                ushort? shortUuid = service.serviceUuid.isShortUuid
                                    ? service.serviceUuid.shortUuid
                                    : (ushort?)null;

                                yield return new BluetoothService(service.attributeHandle, service.serviceUuid.longUuid, shortUuid);
                            }
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
            using (var fileHandle = Interop.CreateFile(serviceFile, Interop.GENERIC_WRITE | Interop.GENERIC_READ, 0,
                IntPtr.Zero, Interop.OPEN_EXISTING, Interop.FILE_ATTRIBUTE_NORMAL, IntPtr.Zero))
            {
                ushort serviceBufferCount;
                Interop.BluetoothGATTGetServices(fileHandle, 0, IntPtr.Zero, out serviceBufferCount, 0);
                
                var serviceSize = Marshal.SizeOf(typeof(BTH_LE_GATT_SERVICE));
                var serviceBufferLength = serviceSize * serviceBufferCount;
                
                var serviceBuffer = Marshal.AllocHGlobal(serviceBufferLength);

                try
                {
                    Interop.BluetoothGATTGetServices(fileHandle, (ushort)serviceBufferLength, serviceBuffer,
                        out serviceBufferCount, 0);

                    for (var i = 0; i < serviceBufferCount; i++)
                    {
                        var servicePtr = IntPtr.Add(serviceBuffer, serviceSize * i);

                        var service =
                            (BTH_LE_GATT_SERVICE)Marshal.PtrToStructure(servicePtr, typeof(BTH_LE_GATT_SERVICE));

                        yield return service;
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(serviceBuffer);
                }
            }
        }
    }
}
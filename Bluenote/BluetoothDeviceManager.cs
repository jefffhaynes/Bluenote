using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace BluetoothGATTInterop
{
    public static class BluetoothDeviceManager
    {
        private static readonly Guid BluetoothServiceClassId = new Guid("e0cbf06c-cd8b-4647-bb8a-263b43f0f974");
        private const int ErrorInsufficientBuffer = 122;

        public static IEnumerable<string> GetDevices()
        {
            return GetDevices(BluetoothServiceClassId);
        }

        public static IEnumerable<string> GetDevices(Guid serviceClass)
        {
            DeviceInfoSetSafeHandle deviceInfoSet = Interop.SetupDiGetClassDevs(
                ref serviceClass, IntPtr.Zero, IntPtr.Zero,
                DiGetClassFlags.DIGCF_PRESENT);
            
            SP_DEVINFO_DATA deviceInfoData = new SP_DEVINFO_DATA();
            deviceInfoData.cbSize = (uint)Marshal.SizeOf(deviceInfoData);

            for (uint i = 0; Interop.SetupDiEnumDeviceInfo(deviceInfoSet, i, ref deviceInfoData); i++)
            {
                // loop on SetupDiGetDeviceRegistryProperty until we have enough space for returned property
                uint propertyRegDataType;
                byte[] propertyBuffer = new byte[0];
                uint propertyBufferSize;
                while (!Interop.SetupDiGetDeviceRegistryProperty(deviceInfoSet, ref deviceInfoData,
                        SetupDiGetDeviceRegistryProperty.SPDRP_DEVICEDESC, out propertyRegDataType, propertyBuffer,
                        (uint)propertyBuffer.Length, out propertyBufferSize))
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

            SP_DEVICE_INTERFACE_DATA devInterfaceData = new SP_DEVICE_INTERFACE_DATA();
            devInterfaceData.cbSize = Marshal.SizeOf(devInterfaceData);

            var spDevinfoData = new SP_DEVINFO_DATA();
            bool initialized = Interop.SetupDiEnumDeviceInterfaces(deviceInfoSet, ref spDevinfoData, ref serviceClass, 0,
                    ref devInterfaceData);
            // I assume The DevInterfaceData is populated correctly as it matches the C Code
            // And I've compared the values in memory and they match

            //uint bytesReturned;
            //initialized = Interop.SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref devInterfaceData, IntPtr.Zero, 0, out bytesReturned, IntPtr.Zero);
            //// I expect bytesReturned = 83 and initialized = true which is the value that is returned in the C Code
            //// Instead the value 162 is returned
        }
    }
}

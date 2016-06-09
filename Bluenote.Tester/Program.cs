using System;
using BluetoothGATTInterop;

namespace BluetoothGATT.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var devices = BluetoothDeviceManager.GetDevices();

            foreach(var device in devices)
                Console.WriteLine(device);

            Console.ReadKey();
        }
    }
}

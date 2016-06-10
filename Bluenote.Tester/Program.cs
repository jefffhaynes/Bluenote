using System;

namespace Bluenote.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            PrintInterfaces();

            Console.ReadKey();
        }

        static void PrintDevices()
        {
            var devices = BluetoothDeviceManager.GetDevices();

            foreach (var device in devices)
                Console.WriteLine(device);
        }

        static void PrintInterfaces()
        {
            var devices = BluetoothDeviceManager.GetDeviceInterfaces();

            foreach (var device in devices)
                Console.WriteLine(device);
        }
    }
}

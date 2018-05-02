using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using InTheHand.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KonnectUI.Common;
using static KonnectUI.Common.Deligates;
using System.Windows;
using System.IO;
using System.Threading;

namespace KonnectUI.Entities.Bluetooth
{
    class BluetoothManager : Source 
    {
        private UUIDEntered uUIDEntered;
        private BluetoothClient bluetoothClient;
        private BluetoothDeviceInfo selectedDevice;
        private Stream bluetoothStream;

        public BluetoothManager()
        {
            Type = "Bluetooth";
            uUIDEntered = ConnectService;
        }
        
        public BluetoothDeviceInfo SelectDevice()
        {
            SelectBluetoothDeviceDialog dialog = new SelectBluetoothDeviceDialog();
            var result = dialog.ShowDialog();
            return dialog.SelectedDevice;
        }

        private bool PairDevice(BluetoothDeviceInfo bluetoothDeviceInfo)
        {
            Status = "Paired";
            if (bluetoothDeviceInfo.Authenticated || bluetoothDeviceInfo.Connected)
            {
                return true;
            }
            else
            {
                BluetoothSecurity.PairRequest(bluetoothDeviceInfo.DeviceAddress, "I5CHK");
                if(bluetoothDeviceInfo.Authenticated)
                {
                    return true;
                }
            }
            return false;
        }

        public override void Connect()
        {
            selectedDevice = SelectDevice();
            Name = selectedDevice.DeviceName;
            OnConnect(this, new ConnectEventArgs("Success"));

        }

        public override void BeginReading()
        {
            BLServices bLServices = new BLServices(uUIDEntered);
            bLServices.Owner = (MainWindow)Application.Current.MainWindow;
            bLServices.ShowDialog();
        }

        public void ConnectService(String uUID)
        {
            Task connectClient = new Task(() =>
            {
                Console.WriteLine($"Attempting Connection with {selectedDevice.DeviceName}");
                try
                {
                    bluetoothClient = new BluetoothClient();
                    bluetoothClient.Connect(selectedDevice.DeviceAddress, new Guid(uUID));
                    if (bluetoothClient.Connected)
                    {
                        Status = "Connected";
                        Console.WriteLine("----Connected----");
                        Task readStream = new Task(() => ReadStream());
                        readStream.Start();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("----Exception Start----");
                    Console.WriteLine(e.Message);
                    Console.WriteLine("----Exception End----");
                }

            });
            connectClient.Start();
        }

        private void ReadStream()
        {
            bluetoothStream = bluetoothClient.GetStream();
            Status = "Reading";

            byte[] fromBluetooth = new byte[1024];
            while (bluetoothStream.CanRead)
            {
                Array.Clear(fromBluetooth, 0, 1024);
                bluetoothStream.Read(fromBluetooth, 0, 1024);
                Publish("/i5/bluetooth/" + Index, Encoding.ASCII.GetString(fromBluetooth).Trim());
                //Console.WriteLine(Encoding.ASCII.GetString(fromBluetooth));
            }
        }
    }
}

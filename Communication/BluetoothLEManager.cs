using KonnectUI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace KonnectUI.Entities.Bluetooth
{
    class BluetoothLEManager : Source
    {
        private GattCharacteristic characteristic;

        public BluetoothLEManager()
        {
            Type = "BluetoothLE";
            
        }

        public override void Connect()
        {
            BluetoothLEDeviceSelection bluetoothLEDeviceSelection = new BluetoothLEDeviceSelection();
            bluetoothLEDeviceSelection.Owner = (MainWindow)Application.Current.MainWindow;
            bluetoothLEDeviceSelection.ValueChanged += Characteristic_ValueChanged;
            bluetoothLEDeviceSelection.ShowDialog();
        }


        private void BluetoothLEDeviceSelection_OnConnect(object sender, EventArgs e)
        {

        }

        public override void BeginReading()
        {
           
        }

        private void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            Console.WriteLine($"x:{reader.ReadUInt16() / 1000}, y: {reader.ReadUInt16() / 1000}, z: {reader.ReadUInt16() / 1000}");
        }
        
    }
}

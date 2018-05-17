using KonnectUI.Common;
using KonnectUI.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace KonnectUI.Entities
{
    class BluetoothLEManager : Source
    {
        private DeviceInformation selectedDeviceInformation;
        private GattDeviceService selectedService;
        private GattCharacteristic selectedCharacteristic;

        private BluetoothLE bluetoothLE;
        Enuminator enuminator;
        MQTTManager mQTTManager;

      

        public BluetoothLEManager()
        {
            Type = "Bluetooth LE";
        }

        public override void Connect()
        {
            bluetoothLE = new BluetoothLE();
            ShowDevices();
        }

        public async void ShowDevices()
        {
            List<Entity> listDevices = new List<Entity>();
            DeviceInformationCollection devices = await bluetoothLE.GetSensors();
            foreach (var deviceInformation in devices)
            {

                listDevices.Add(new Entity(deviceInformation.Name, typeof(DeviceInformation), deviceInformation));

            }
            enuminator = new Enuminator("Bluetooth LE Devices", listDevices, OnDeviceSelected);
            enuminator.ShowDialog();
        }

        private void OnDeviceSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                enuminator.ListTitle = "Bluetooth LE Services";
                selectedDeviceInformation = (DeviceInformation)((Entity)e.AddedItems[0]).Item;
                ShowServices(selectedDeviceInformation);
                Console.WriteLine(selectedDeviceInformation.Name);
            }
        }

        public async void ShowServices(DeviceInformation deviceInformation)
        {
            List<Entity> listServices = new List<Entity>();
            var services = await bluetoothLE.GetServices(deviceInformation);
            if (services != null)
            {
                foreach (var service in services)
                {
                    listServices.Add(new Entity(service.Uuid.ToString(), typeof(GattDeviceService), service));
                }
                enuminator.listItems.ItemsSource = listServices;
                Address = "/i5/ble/" + Index;
                enuminator.listItems.SelectionChanged -= OnDeviceSelected;
                enuminator.listItems.SelectionChanged += OnServiceSelected;
            }
            else
            {
                Console.WriteLine("No Services found");
            }
        }

        private void OnServiceSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                selectedService = (GattDeviceService)((Entity)e.AddedItems[0]).Item;
                enuminator.ListTitle = selectedService.Uuid.ToString() + " Characteristics";
                ShowCharacteristics(selectedService);
            }
        }

        public void ShowCharacteristics(GattDeviceService service)
        {
            List<Entity> listCharacteristics = new List<Entity>();
            var characteristics = bluetoothLE.GetCharacteristics(service);
            if (characteristics != null)
            {
                foreach (var characteristic in characteristics)
                {
                    listCharacteristics.Add(new Entity(characteristic.Uuid.ToString(), typeof(GattCharacteristic), characteristic));
                }
                enuminator.listItems.ItemsSource = listCharacteristics;
                enuminator.listItems.SelectionChanged -= OnServiceSelected;
                enuminator.listItems.SelectionChanged += OnCharacteristicSelected;
            }
            else
            {
                Console.WriteLine("No Services found");
            }
        }

        private void OnCharacteristicSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                selectedCharacteristic = (GattCharacteristic)((Entity)e.AddedItems[0]).Item;
                if (selectedCharacteristic != null)
                {
                    enuminator.Hide();
                    enuminator = null;
                    Status = "Connected";
                    OnConnect(this, new ConnectEventArgs("Success"));
                }
            }
        }

        public async override void BeginReading()
        {
            Boolean response = await bluetoothLE.PerformAction(selectedCharacteristic, ValueChanged);
            Status = "Transmiting";
        }

        private void ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            Console.WriteLine(reader.ToString());
            //Publish("/i5/ble/" + Index, $"{reader.ReadUInt16()},{reader.ReadUInt16()},{reader.ReadUInt16()}");
        }
    }
}

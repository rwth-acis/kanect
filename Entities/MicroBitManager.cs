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
    class MicroBitManager : Source
    {
        private DeviceInformation selectedDeviceInformation;
        private GattDeviceService selectedService;
        private GattCharacteristic selectedCharacteristic;

        private BluetoothLE bluetoothLE;
        Enuminator enuminator;
        MQTTManager mQTTManager;

        private static Dictionary<String, Guid> UUIDS = new Dictionary<string, Guid>()
        {
            {"Accelerometer Service", new Guid("E95D0753251D470AA062FA1922DFA9A8") },
            { "Accelerometer Data", new Guid("E95DCA4B251D470AA062FA1922DFA9A8")},
            {"Accelerometer Period", new Guid("E95DFB24251D470AA062FA1922DFA9A8")},
            {"Magnetometer Service", new Guid("E95DF2D8251D470AA062FA1922DFA9A8") },
            {"Magnetometer Data", new Guid("E95DFB11251D470AA062FA1922DFA9A8")},
            {"Magnetometer Period", new Guid("E95D386C251D470AA062FA1922DFA9A8")},
            {"Magnetometer Bearing", new Guid("E95D9715251D470AA062FA1922DFA9A8")},
            { "Button Service", new Guid("E95D9882251D470AA062FA1922DFA9A8")},
            {"Button A State", new Guid("E95DDA90251D470AA062FA1922DFA9A8")},
            {"Button B State", new Guid("E95DDA91251D470AA062FA1922DFA9A8")},
            {"Event Service", new Guid("E95D93AF251D470AA062FA1922DFA9A8") },
            {"Temperature Service", new Guid("E95D6100251D470AA062FA1922DFA9A8") },
            {"Temperature Data", new Guid("E95D9250251D470AA062FA1922DFA9A8")},
            {"Temperature Period", new Guid("E95D1B25251D470AA062FA1922DFA9A8")}
        };

        public static string UUIDToName(Guid guid)
        {
            var foundedService = UUIDS.FirstOrDefault(t => t.Value.Equals(guid));
            return foundedService.Key;
        }

        public static Guid NameToUUID(String name)
        {
            var founded = UUIDS.FirstOrDefault(t => t.Key.Equals(name));
            return founded.Value;
        }

        public MicroBitManager()
        {
            Type = "BBC MicroBit";
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
                if (deviceInformation.Name.Contains("BBC micro"))
                {
                    listDevices.Add(new Entity(deviceInformation.Name, typeof(DeviceInformation), deviceInformation));
                }
            }
            enuminator = new Enuminator("BBC MicroBit Devices", listDevices, OnDeviceSelected);
            enuminator.ShowDialog();
        }

        private void OnDeviceSelected(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0)
            {
                enuminator.ListTitle = "MicroBit Services";
                selectedDeviceInformation = (DeviceInformation)((Entity)e.AddedItems[0]).Item;
                ShowServices(selectedDeviceInformation);
                Console.WriteLine(selectedDeviceInformation.Name);
            }
        }

        public async void ShowServices(DeviceInformation deviceInformation)
        {
            List<Entity> listServices = new List<Entity>();
            var services = await bluetoothLE.GetServices(deviceInformation);
            if(services != null)
            {
                foreach (var service in services)
                {
                    if (MicroBits.UUIDToName(service.Uuid) != null)
                    {
                        listServices.Add(new Entity(MicroBits.UUIDToName(service.Uuid), typeof(GattDeviceService), service));
                    }
                }
                enuminator.listItems.ItemsSource = listServices;
                enuminator.listItems.SelectionChanged -= OnDeviceSelected;
                enuminator.listItems.SelectionChanged += OnServiceSelected;
            } else
            {
                Console.WriteLine("No Services found");
            }
        }

        private void OnServiceSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                selectedService = (GattDeviceService)((Entity)e.AddedItems[0]).Item;
                enuminator.ListTitle = MicroBits.UUIDToName(selectedService.Uuid) + " Characteristics";
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
                    if (MicroBits.UUIDToName(characteristic.Uuid) != null)
                    {
                        listCharacteristics.Add(new Entity(MicroBits.UUIDToName(characteristic.Uuid), typeof(GattCharacteristic), characteristic));
                    }
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
            if(e.AddedItems.Count > 0)
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
            Publish("/i5/micro:bit/" + Index, $"x:{reader.ReadUInt16() / 1000}, y: {reader.ReadUInt16() / 1000}, z: {reader.ReadUInt16() / 1000}");
        }
    }
}

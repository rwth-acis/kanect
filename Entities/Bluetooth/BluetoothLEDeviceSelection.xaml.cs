using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using System.Threading;
using KonnectUI.Common;
using Windows.Foundation;

namespace KonnectUI.Entities.Bluetooth
{
    /// <summary>
    /// Interaction logic for BluetoothLEDeviceSelection.xaml
    /// </summary>
    public partial class BluetoothLEDeviceSelection : Window
    {
        private List<DeviceInformation> foundedDevices = new List<DeviceInformation>();
        private DeviceWatcher deviceWatcher;
        private DeviceInformation selectedDeviceInformation;
        private BluetoothLEDevice selectedBluetoothLEDevice = null;
        private GattCharacteristic characteristic;

        public event TypedEventHandler<GattCharacteristic, GattValueChangedEventArgs> ValueChanged;

        string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected", "System.Devices.Aep.Bluetooth.Le.IsConnectable" };

        public BluetoothLEDeviceSelection()
        {
            
            InitializeComponent();
            startWatcher();
            listDevices.ItemsSource = foundedDevices;
        }

        private void startWatcher()
        {
            deviceWatcher =
                    DeviceInformation.CreateWatcher(
                                BluetoothLEDevice.GetDeviceSelectorFromPairingState(true),
                                requestedProperties,
                                DeviceInformationKind.AssociationEndpoint);

            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Updated -= DeviceWatcher_Updated;
            deviceWatcher.Removed += DeviceWatcher_Removed;
            deviceWatcher.Start();
        }

        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceUpdateInfo)
        {
            Dispatcher.Invoke(() =>
            {
                DeviceInformation foundedDevice = FindDevices(deviceUpdateInfo.Id);
                if (foundedDevice != null)
                {
                    foundedDevice.Update(deviceUpdateInfo);
                }
                listDevices.ItemsSource = foundedDevices;

            });
        }

        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            Dispatcher.Invoke(() =>
            {
                foundedDevices.Add(deviceInfo);
                listDevices.ItemsSource = foundedDevices;
            });
        }

        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate deviceUpdateInfo)
        {
            Dispatcher.Invoke(() =>
            {
                DeviceInformation foundedDevice = FindDevices(deviceUpdateInfo.Id);
                if (foundedDevice != null)
                {
                    foundedDevices.Remove(foundedDevice);
                }
            });
        }
    
        private void ListDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedDeviceInformation = (DeviceInformation)listDevices.SelectedItem;
            if (selectedDeviceInformation.Pairing.IsPaired)
            {
                loadServiceList();
            } else
            {
                Console.WriteLine("Unable to pair");
            }
        }

        private DeviceInformation FindDevices(string id)
        {
            foreach (DeviceInformation bleDeviceInfo in foundedDevices)
            {
                if (bleDeviceInfo.Id == id)
                {
                    return bleDeviceInfo;
                }
            }
            return null;
        }

        private async void loadServiceList()
        {
            selectedBluetoothLEDevice = await BluetoothLEDevice.FromIdAsync(selectedDeviceInformation.Id);

            var services = selectedBluetoothLEDevice.GattServices;

            if (services.Count > 0)
            {
                foreach (var service in services)
                {
                    if(MicroBits.UUIDToName(service.Uuid) != null)
                    {
                        listServices.Items.Add(service);
                    }
                }
                showServiceList();
            } else
            {
                Console.WriteLine("No services found");
            }
        }

        private void showServiceList()
        {
            listDevices.Visibility = Visibility.Collapsed;
            listServices.Visibility = Visibility.Visible;
        }

        private void ListServices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GattDeviceService service = (GattDeviceService)listServices.SelectedItem;
            var characteristics = service.GetAllCharacteristics();
            if(characteristics.Count > 0)
            {
                foreach(var characteristic in characteristics)
                {
                    if (MicroBits.UUIDToName(characteristic.Uuid) != null)
                    {
                        listCharacteristic.Items.Add(characteristic);
                    }
                    
                }
                showChracteristicsList();
            }
            else
            {
                Console.WriteLine("No services found");
            }
            
        }

        private void showChracteristicsList()
        {
            listServices.Visibility = Visibility.Collapsed;
            listCharacteristic.Visibility = Visibility.Visible;
        }

        private async void ListCharacteristic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            characteristic = (GattCharacteristic)listCharacteristic.SelectedItem;
            var currentDescriptorValue = await characteristic.ReadClientCharacteristicConfigurationDescriptorAsync();
            GattCharacteristicProperties properties = characteristic.CharacteristicProperties;
            if (properties.HasFlag(GattCharacteristicProperties.Write))
            {
                Console.WriteLine($"{characteristic.Uuid} Can Write");
            }


            if (properties.HasFlag(GattCharacteristicProperties.Read))
            {
                Console.WriteLine($"{characteristic.Uuid} Can Read");
            }
            
            if (properties.HasFlag(GattCharacteristicProperties.Notify))
            {
                Console.WriteLine($"{characteristic.Uuid} Can Notify");
                GattCommunicationStatus status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                if (status == GattCommunicationStatus.Success)
                {
                    // Server has been informed of clients interest.
                    characteristic.ValueChanged += ValueChanged;
                    Console.WriteLine($"{characteristic.Uuid} will notify");
                }

            }
        }

        private void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            Console.WriteLine($"x:{reader.ReadUInt16() / 1000}, y: {reader.ReadUInt16() / 1000}, z: {reader.ReadUInt16() / 1000}");
        }
    }
}

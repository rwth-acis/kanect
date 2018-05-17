using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;

namespace KonnectUI.Communication
{
    class BluetoothLE
    {
        public enum CharacteristicAction { Read, Write, Notify };
        private List<DeviceInformation> foundedDevices = new List<DeviceInformation>();
        string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected", "System.Devices.Aep.Bluetooth.Le.IsConnectable" };
        private DeviceWatcher deviceWatcher;

        public EventHandler<List<DeviceInformation>> OnSensorSearchComplete;

        public async Task<DeviceInformationCollection> GetSensors()
        {
            var collection = await DeviceInformation.FindAllAsync(BluetoothLEDevice.GetDeviceSelectorFromPairingState(true));
            return collection;
        }

        public async Task<IReadOnlyList<GattDeviceService>> GetServices(DeviceInformation deviceInformation)
        {
            var device = await BluetoothLEDevice.FromIdAsync(deviceInformation.Id);
            if (device != null)
            {
                return device.GattServices;
            }
            else
            {
                return null;
            }
        }

        public IReadOnlyList<GattCharacteristic> GetCharacteristics(GattDeviceService service)
        {
            try
            {
                return service.GetAllCharacteristics();
            }
            catch (System.IO.FileLoadException)
            {
                MessageBox.Show("Seems like Device is already in use.", "Wait Sparky!!!");
            }

            return Array.Empty<GattCharacteristic>();
        }

        public async Task<Boolean> PerformAction(GattCharacteristic characteristic, TypedEventHandler<GattCharacteristic, GattValueChangedEventArgs> OnValueChanged)
        {
            var currentDescriptorValue = await characteristic.ReadClientCharacteristicConfigurationDescriptorAsync();
            GattCharacteristicProperties properties = characteristic.CharacteristicProperties;

            if (properties.HasFlag(GattCharacteristicProperties.Notify))
            {
                GattCommunicationStatus status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                if (status == GattCommunicationStatus.Success)
                {
                    characteristic.ValueChanged += OnValueChanged;
                    return true;
                }
            }
            return true;
        }

        public void SearchSensor()
        {
            deviceWatcher =
                    DeviceInformation.CreateWatcher(
                                BluetoothLEDevice.GetDeviceSelectorFromPairingState(false),
                                requestedProperties,
                                DeviceInformationKind.AssociationEndpoint);

            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Updated -= DeviceWatcher_Updated;
            deviceWatcher.Removed += DeviceWatcher_Removed;
            deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            deviceWatcher.Start();
        }

        private void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            OnSensorSearchComplete(this, foundedDevices);
        }

        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceUpdateInfo)
        {
            DeviceInformation foundedDevice = FindDevices(deviceUpdateInfo.Id);
            if (foundedDevice != null)
            {
                foundedDevice.Update(deviceUpdateInfo);
            }
        }

        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            foundedDevices.Add(deviceInfo);
        }

        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate deviceUpdateInfo)
        {
            DeviceInformation foundedDevice = FindDevices(deviceUpdateInfo.Id);
            if (foundedDevice != null)
            {
                foundedDevices.Remove(foundedDevice);
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


    }
}

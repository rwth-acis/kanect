using InTheHand.Net.Sockets;
using InTheHand.Windows.Forms;
using InTheHand.Net.Bluetooth;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using KonnectUI.Entities;
using System.IO;
using KonnectUI.Common;
using KonnectUI.Entities.Bluetooth;
using KonnectUI.Entities;
using MyoSharp.Communication;
using MyoSharp.Device;
using MyoSharp.Exceptions;

namespace KonnectUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Source> addedDevices = new List<Source>();
        private BluetoothManager bluetoothManager;
        private short microBitIndex = 0;

        public Enuminator Enuminator { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            MQTTManager.TestConnection();            
        }

        private void Bluetooth_DeviceConnnect(object sender, RoutedEventArgs e)
        {
            /*BluetoothManager bluetoothManager = new BluetoothManager();
            if (bluetoothManager.Connect())
            {
                addedDevices.Add(bluetoothManager);
            }

            listDevices.ItemsSource = addedDevices;*/

            //BluetoothLEManager bluetoothLEManager = new BluetoothLEManager();
            //bluetoothLEManager.Connect();

        }

        private void ListDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            addedDevices[listDevices.SelectedIndex].BeginReading();
            addedDevices[listDevices.SelectedIndex].Status = "Transmitting";
            addedDevices[listDevices.SelectedIndex].Index = listDevices.SelectedIndex.ToString();
            listDevices.ItemsSource = addedDevices;
            listDevices.Items.Refresh();
        }

        private void AddBluetoothLE(object sender, RoutedEventArgs e)
        {
            BluetoothLEManager bluetoothLEManager = new BluetoothLEManager();
            bluetoothLEManager.OnConnect += OnBluetoothLEConnect;
            bluetoothLEManager.Connect();
        }

        private void OnBluetoothLEConnect(object sender, ConnectEventArgs e)
        {
            if (e.Status == "Success")
            {
                var bluetoothLEManager = (BluetoothLEManager)sender;
                bluetoothLEManager.Index = microBitIndex.ToString();
                bluetoothLEManager.Address += bluetoothLEManager.Index;
                Console.WriteLine(bluetoothLEManager.Address);
                addedDevices.Add(bluetoothLEManager);
                listDevices.ItemsSource = addedDevices;
                listDevices.Items.Refresh();
            }
            else
            {
                Console.WriteLine(e.Message);
            }
        }

        private void AddKinect(object sender, RoutedEventArgs e)
        {
            KinectManager kinectManager = new KinectManager();
            kinectManager.OnConnect += OnKinectConnect;
            kinectManager.Connect();
        }

        private void OnKinectConnect(object sender, ConnectEventArgs e)
        {
            if (e.Status == "Success")
            {
                var kinectManager = (KinectManager)sender;
                kinectManager.BeginReading();
                addedDevices.Add(kinectManager);
                listDevices.ItemsSource = addedDevices;
                listDevices.Items.Refresh();
            }
            else
            {
                Console.WriteLine(e.Message);
            }
        }
        
        private void AddMicroBit(object sender, RoutedEventArgs e)
        {
            MicroBitManager microBit = new MicroBitManager();
            microBit.OnConnect += OnMicroBitConnect;
            microBit.Connect();
        }

        private void OnMicroBitConnect(object sender, ConnectEventArgs e)
        {
            if (e.Status == "Success")
            {
                var microBitManager = (MicroBitManager)sender;
                microBitManager.Index = microBitIndex.ToString();
                microBitManager.Address += microBitManager.Index;
                Console.WriteLine(microBitManager.Address);
                addedDevices.Add(microBitManager);
                listDevices.ItemsSource = addedDevices;
                listDevices.Items.Refresh();
            }
            else
            {
                Console.WriteLine(e.Message);
            }
        }

        private void AddBluetooth(object sender, RoutedEventArgs e)
        {
            bluetoothManager = new BluetoothManager();
            bluetoothManager.OnConnect += OnBluetoothConnect;
            bluetoothManager.Connect();
        }

        private void OnBluetoothConnect(object sender, ConnectEventArgs e)
        {
            addedDevices.Add(bluetoothManager);
            listDevices.ItemsSource = addedDevices;
            listDevices.Items.Refresh();
        }

        private void AddMyo(object sender, RoutedEventArgs e)
        {

            MyoManager myoManager = new MyoManager();
            myoManager.OnConnect += OnMyoConnect;
            myoManager.Connect();

        }

        private void OnMyoConnect(object sender, ConnectEventArgs e)
        {

            if (e.Status == "Success")
            {
                var myoManager = (MyoManager)sender;
                addedDevices.Add(myoManager);
                //listDevices.ItemsSource = addedDevices;
                //listDevices.Items.Refresh();
                myoManager.BeginReading();
            }
            else
            {
                Console.WriteLine(e.Message);
            }


        }
    }
}

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
using System.Threading;

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
        private Timer SensorClock;
        private int Tick = 0;
        private int Threashold = 2;
        private bool isMyoExists = false;

        public Enuminator Enuminator { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            MQTTManager.TestConnection();
            SensorClock = new Timer(new TimerCallback((state) =>
            {
                ++Tick;
                if (addedDevices.Count > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        for(var i = 0; i < addedDevices.Count; i++)
                        {
                            addedDevices[i].CurrentTick = Tick;
                            Console.WriteLine(Tick.ToString() + "::" + addedDevices[i].LastTransmissionTick.ToString());
                            if (Tick - addedDevices[i].LastTransmissionTick > Threashold && addedDevices[i].Status == "Transmitting")
                            {
                                EndTransmission(i, "Paused");
                            }else if(Tick - addedDevices[i].LastTransmissionTick > Threashold && addedDevices[i].Status == "Paused")
                            {
                                addedDevices[i].BeginReading();
                            } else if(Tick - addedDevices[i].LastTransmissionTick <= Threashold && addedDevices[i].Status == "Paused")
                            {
                                StartTransmission(i, "Transmitting");
                            }
                        }
                    });
                }
            }));

            SensorClock.Change(0, 5000);
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
            
        }

        private void listDevices_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (addedDevices.Count > 0 && listDevices.SelectedIndex >= 0)
            {
                if(addedDevices[listDevices.SelectedIndex].Status == "Transmitting" || addedDevices[listDevices.SelectedIndex].Status == "Paused")
                {
                    EndTransmission(listDevices.SelectedIndex, "Connected");

                } else
                {
                    StartTransmission(listDevices.SelectedIndex, "Transmitting");
                }
                
            }
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
            myoManager.OnDisconnect += OnMyoDisconnect;
            myoManager.Connect();

        }

        private void OnMyoDisconnect(object sender, ConnectEventArgs e)
        {
            if (e.Status == "Success")
            {
                /*Dispatcher.Invoke(() =>
                {
                    var myoManager = (MyoManager)sender;
                    addedDevices.Remove(myoManager);
                    myoManager.Status = "Paused";
                    listDevices.ItemsSource = addedDevices;
                    listDevices.Items.Refresh();

                });*/
            }
            else
            {
                Console.WriteLine(e.Message);
            }
        }

        private void OnMyoConnect(object sender, ConnectEventArgs e)
        {

            if (e.Status == "Success")
            {
                Dispatcher.Invoke(() =>
                {
                    if (isMyoExists == true)
                    {
                        for(int i = 0; i < addedDevices.Count; i++)
                        {
                            if(addedDevices[i].Type == "Myo")
                            {
                                StartTransmission(i, "Transmitting");
                            }
                        }
                    } else
                    {
                        var myoManager = (MyoManager)sender;
                        myoManager.CurrentTick = Tick;
                        addedDevices.Add(myoManager);
                        listDevices.ItemsSource = addedDevices;
                        listDevices.Items.Refresh();
                    }
                    isMyoExists = true;
                });
                //myoManager.BeginReading();
            }
            else
            {
                Console.WriteLine(e.Message);
            }


        }

        private void StartTransmission(int index, string status)
        {
            addedDevices[index].CurrentTick = Tick;
            addedDevices[index].BeginReading();
            ChangeStatus(index, status);
        }

        private void EndTransmission(int index, string status)
        {
            addedDevices[index].EndReading();
            ChangeStatus(index, status);
        }

        private void ChangeStatus(int index, string status)
        {
            addedDevices[index].Status = status;
            addedDevices[index].Index = listDevices.SelectedIndex.ToString();
            listDevices.ItemsSource = addedDevices;
            listDevices.Items.Refresh();
        }
    }
}

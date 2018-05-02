using KonnectUI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Microsoft.Kinect;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace KonnectUI.Entities
{
    class KinectManager : Source
    {
        KinectSensor kinectSensor;
        Enuminator enuminator;
        DepthImagePixel[] currntDepthData;

        public KinectManager()
        {
            Type = "Kinect";
        }
        public override void Connect()
        {
            List<Entity> listDevices = new List<Entity>();
          
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                listDevices.Add(new Entity(potentialSensor.UniqueKinectId, typeof(KinectSensor), potentialSensor));
            }

            enuminator = new Enuminator("Kinect Devices", listDevices, DeviceSelected);
            enuminator.ShowDialog();
        }

        public void test()
        {
            var kinectSensor = KinectSensor.KinectSensors[0];
            kinectSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            kinectSensor.DepthFrameReady += FrameReady;
            kinectSensor.Start();
        }

        void FrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            var imageFrame = e.OpenDepthImageFrame();
            if (imageFrame != null)
            {
                Console.WriteLine(imageFrame);
            }
        }

        private void DeviceSelected(object sender, SelectionChangedEventArgs e)
        {
            enuminator.Hide();
            if (e.AddedItems.Count > 0)
            {
                kinectSensor = (KinectSensor)((Entity)e.AddedItems[0]).Item;
                if (kinectSensor.Status == KinectStatus.Connected)
                {
                    Name = kinectSensor.UniqueKinectId;
                    OnConnect(this, new ConnectEventArgs("Success"));
                    return;
                }
            }
            else
            {
                OnConnect(this, new ConnectEventArgs("Error", "No Kinect Devices Selected."));
                return;
            }
            OnConnect(this, new ConnectEventArgs("Error", "Unable to connect to Kinect."));
        }

        public override void BeginReading()
        {
            kinectSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            kinectSensor.DepthFrameReady += SensorDepthFrameReady;
            kinectSensor.Start();
            Console.WriteLine($"Kinect is {kinectSensor.Status} and {kinectSensor.IsRunning}");
        }

        private void SensorDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {
                    short[] pixelsFromFrame = new short[depthFrame.PixelDataLength];
                    depthFrame.CopyPixelDataTo(pixelsFromFrame);
                    Publish("/i5/kinect/" + Index, $"pixelCount:{pixelsFromFrame.Length}");
                }
            }
        }
    }
}

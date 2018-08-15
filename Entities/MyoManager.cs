using KonnectUI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using MyoSharp.Communication;
using MyoSharp.Device;
using MyoSharp.Exceptions;

namespace KonnectUI.Entities
{
    class MyoManager : Source
    {
        string Acc, Gyro, EMG;

        IChannel myoChannel;
        IHub myoHub;
        bool streamData = false;

        public MyoManager()
        {
            Type = "Myo";
            myoChannel = Channel.Create(ChannelDriver.Create(ChannelBridge.Create()));
            myoHub = Hub.Create(myoChannel);

            myoHub.MyoConnected += MyoConnected;
            myoHub.MyoDisconnected += MyoDisconnected;

            
        }

        private void MyoDisconnected(object sender, MyoEventArgs e)
        {
            Publish("/i5/myo/pose", "end");
            Publish("/i5/myo/orientation", "end");
            Publish("/i5/myo/full", "end");
            Publish("/i5/myo/gyroscope", "end");
            Publish("/i5/myo/accelerometer", "end");
            Publish("/i5/myo/emg", "end");
            OnDisconnect(this, new ConnectEventArgs("Success"));
            streamData = false;
        }

        private void MyoConnected(object sender, MyoEventArgs e)
        {
            Status = "Connected";
            e.Myo.AccelerometerDataAcquired += AccelerometerDataAcquired;
            e.Myo.SetEmgStreaming(true);
            e.Myo.EmgDataAcquired += EmgDataAcquired;
            e.Myo.GyroscopeDataAcquired += GyroscopeDataAcquired;
            e.Myo.OrientationDataAcquired += OrientationDataAcquired;
            e.Myo.PoseChanged += PoseChanged;
            OnConnect(this, new ConnectEventArgs("Success"));
        }

        private void PoseChanged(object sender, PoseEventArgs e)
        {
            if(streamData == true)
            {
                Publish("/i5/myo/pose", e.Myo.Pose.ToString());
            }
        }

        private void OrientationDataAcquired(object sender, OrientationDataEventArgs e)
        {
            if (streamData == true)
            {
                const float PI = (float)System.Math.PI;

                var roll = (int)((e.Roll + PI) / (PI * 2.0f) * 10);
                var pitch = (int)((e.Pitch + PI) / (PI * 2.0f) * 10);
                var yaw = (int)((e.Yaw + PI) / (PI * 2.0f) * 10);

                Publish("/i5/myo/orientation", roll.ToString() + "," + pitch.ToString() + "," + yaw.ToString());
            }
        }

        private void GyroscopeDataAcquired(object sender, GyroscopeDataEventArgs e)
        {
            LastTransmissionTick = CurrentTick;
            if (streamData == true)
            {
                //"," + DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() + "," 
                Publish("/i5/myo/full", EMG + Acc + "," + e.Gyroscope.X + "," + e.Gyroscope.Y.ToString() + "," + e.Gyroscope.Z.ToString());
                Publish("/i5/myo/gyroscope", e.Gyroscope.X + "," + e.Gyroscope.Y.ToString() + "," + e.Gyroscope.Z.ToString());
            }
        }

        private void EmgDataAcquired(object sender, EmgDataEventArgs e)
        {
            if (streamData == true)
            {
                string tmpEmg = "";
                for (var i = 0; i < 8; ++i)
                {
                    tmpEmg += e.EmgData.GetDataForSensor(i).ToString() + ",";
                }


                EMG = /*DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() +*/ tmpEmg;
                Publish("/i5/myo/emg", tmpEmg.Substring(0, tmpEmg.Length - 1));
            }
        }

        private void AccelerometerDataAcquired(object sender, AccelerometerDataEventArgs e)
        {            
            if (streamData == true)
            {
                Acc = /*DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() + "," +*/ e.Accelerometer.X.ToString() + "," + e.Accelerometer.Y.ToString() + "," + e.Accelerometer.Z.ToString();
                Publish("/i5/myo/accelerometer", e.Accelerometer.X.ToString() + "," + e.Accelerometer.Y.ToString() + "," + e.Accelerometer.Z.ToString());
            }
        }

        public override void Connect()
        {
            myoChannel.StartListening();
        }

        public override void BeginReading()
        {
            streamData = true;
        }

        public override void EndReading()
        {
            Publish("/i5/myo/pose", "end");
            Publish("/i5/myo/orientation", "end");
            Publish("/i5/myo/full", "end");
            Publish("/i5/myo/gyroscope", "end");
            Publish("/i5/myo/accelerometer", "end");
            Publish("/i5/myo/emg", "end");
            streamData = false;
        }
    }
}

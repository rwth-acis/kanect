using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using System.Windows;

namespace KonnectUI.Common
{
    class MQTTManager
    {
        private MqttClient mqttClient;
        private String UUID = "cc36fa72-2af8-4161-9004-a73a0d69aed7";

        public MQTTManager()
        {
            mqttClient = new MqttClient("iot.eclipse.org");
            mqttClient.Connect(Guid.NewGuid().ToString());
        }

        public static void TestConnection()
        {
            try
            {
                MqttClient mqttClient = new MqttClient("iot.eclipse.org");
                mqttClient.Connect(Guid.NewGuid().ToString());
                mqttClient.Disconnect();
            }
            catch (MqttConnectionException)
            {
                MessageBox.Show("You forgot it again don't ya? Start the damn MQTT Server first.", "Wait Sparky!!!");
                System.Windows.Application.Current.Shutdown();
            }

        }

        public void WriteString(String topic, String data)
        {
            mqttClient.Publish(topic, Encoding.UTF8.GetBytes(data));
        }
    }
}

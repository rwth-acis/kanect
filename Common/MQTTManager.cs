using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;


namespace KonnectUI.Common
{
    class MQTTManager
    {
        private MqttClient mqttClient;
        private String UUID = "cc36fa72-2af8-4161-9004-a73a0d69aed7";

        public MQTTManager()
        {
            mqttClient = new MqttClient(IPAddress.Parse("127.0.0.1"));
            mqttClient.Connect(Guid.NewGuid().ToString());
        }

        public void WriteString(String topic, String data)
        {
            mqttClient.Publish(topic, Encoding.UTF8.GetBytes(data));
        }
    }
}

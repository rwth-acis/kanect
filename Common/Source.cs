using KonnectUI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using System.Net;
using System.Threading;

namespace KonnectUI.Common
{
    public abstract class Source
    {

        string _Name, _Type, _Status, _Index, _Address;
        public string Name { get { return _Name; } set { _Name = value; } }
        public string Type { get { return _Type; } set { _Type = value; } }
        public string Status { get { return _Status; } set { _Status = value; } }
        public string Index { get { return _Index; } set { _Index = value; } }
        public string Address { get { return _Address; } set { _Address = value; } }
        public int CurrentTick { get; set; }
        public int LastTransmissionTick { get; set; }

        public EventHandler<ConnectEventArgs> OnConnect;
        public EventHandler<ConnectEventArgs> OnDisconnect;

        MQTTManager mQTTManager;

        abstract public void Connect();
        abstract public void BeginReading();

        abstract public void EndReading();

        public void Publish(String topic, String data)
        {
            mQTTManager.WriteString(topic, data);
        }

        public Source()
        {
            mQTTManager = new MQTTManager();
        }


        

    }
}

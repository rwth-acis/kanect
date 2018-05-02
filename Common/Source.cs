using KonnectUI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using System.Net;

namespace KonnectUI.Common
{
    public abstract class Source
    {

        string _Name, _Type, _Status, _Index;
        public string Name { get { return _Name; } set { _Name = value; } }
        public string Type { get { return _Type; } set { _Type = value; } }
        public string Status { get { return _Status; } set { _Status = value; } }
        public string Index { get { return _Index; } set { _Index = value; } }

        public EventHandler<ConnectEventArgs> OnConnect;

        MQTTManager mQTTManager;

        abstract public void Connect();
        abstract public void BeginReading();

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

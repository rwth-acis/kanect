using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonnectUI.Common
{
    public class ConnectEventArgs : EventArgs
    {
        private String status, message;

        public ConnectEventArgs(String _status)
        {
            status = _status;
        }

        public ConnectEventArgs(String _status, String _message)
        {
            status = _status;
            message = _message;
        }

        public String Status
        {
            get { return status; }
            set { status = value; }
        }

        public String Message
        {
            get { return message; }
            set { message = value; }
        }
    }
}

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
using System.Windows.Shapes;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using KonnectUI.Entities.Bluetooth;
using static KonnectUI.Common.Deligates;

namespace KonnectUI.Entities.Bluetooth
{
    /// <summary>
    /// Interaction logic for BLServices.xaml
    /// </summary>
    public partial class BLServices : Window
    {
        UUIDEntered uuidChanged;

        public BLServices()
        {
            InitializeComponent();
        }

        public BLServices(UUIDEntered uUIDEntered)
        {
            InitializeComponent();
            uuidChanged = uUIDEntered;
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            uuidChanged(txtUUID.Text);
            this.Hide();
        }
    }
}

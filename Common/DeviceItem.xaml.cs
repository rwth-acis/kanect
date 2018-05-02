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
using System.Windows.Controls.Primitives;

namespace KonnectUI.Common
{
    /// <summary>
    /// Interaction logic for DeviceItem.xaml
    /// </summary>
    public partial class DeviceItem : UserControl
    {

        public String Title { get { return MainTitle.Text; } set { MainTitle.Text = value; } }
        public String Icon { get { return MainIcon.Text; } set { MainIcon.Text = value; } }

       public RoutedEventHandler OnButtonClick { get { return null; } set { AddButton.Click += value; } }

        public DeviceItem()
        {
            InitializeComponent();
        }
    }
}

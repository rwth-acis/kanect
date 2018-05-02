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

namespace KonnectUI.Entities
{
    /// <summary>
    /// Interaction logic for Enuminator.xaml
    /// </summary>
    public partial class Enuminator : Window
    {
        public String ListTitle { get { return MainTitle.Text; } set { MainTitle.Text = value; } }


        public Enuminator(String title)
        {
            InitializeComponent();
            MainTitle.Text = title;
        }

        public Enuminator(String title, List<Entity> list, SelectionChangedEventHandler OnSelectionChanged)
        {
            InitializeComponent();
            MainTitle.Text = title;
            listItems.SelectionChanged += OnSelectionChanged;
            listItems.ItemsSource = list;
        }
    }
}

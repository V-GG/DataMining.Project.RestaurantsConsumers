using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BOLZANO.DataMining.RestaurantsAndConsumers
{
    /// <summary>
    /// Interaction logic for RestaurantsAndConsumersMainWindow.xaml
    /// </summary>
    public partial class RestaurantsAndConsumersMainWindow : Window
    {
        public RestaurantsAndConsumersMainWindow()
        {
            InitializeComponent();
            try
            {
               this.DataContext = new RestaurantsAndConsumersViewModel();
            }
            catch (Exception)
            {                
                throw;
            }
            
        }
    }
}

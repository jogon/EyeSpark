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

namespace GazeTrackerUI.Mappings
{
    /// <summary>
    /// Interaction logic for MappingsWindow.xaml
    /// </summary>
    public partial class MappingsWindow : Window
    {
        private static MappingsWindow instance;

        private MappingsWindow()
        {
            InitializeComponent();
        }

        public static MappingsWindow Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MappingsWindow();
                }
                return instance;

            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Collapsed;
        }
    }
}

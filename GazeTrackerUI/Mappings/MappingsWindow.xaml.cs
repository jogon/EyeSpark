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
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace GazeTrackerUI.Mappings
{
    /// <summary>
    /// Interaction logic for MappingsWindow.xaml
    /// </summary>
    public partial class MappingsWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd,
            StringBuilder buffer, int length);

        private static string DefaultApplicationName = "-None Selected-";

        private static MappingsWindow instance;

        private String selectedItem = "";
        private Dictionary<String, String> selectedMap;
        private Dictionary<String, String> dummyMap;

        private MappingsWindow()
        {
            CreateDummyMap();
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

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            appComboBox.Items.Add(DefaultApplicationName);
            appComboBox.SelectedItem = DefaultApplicationName;
            //appComboBox.SelectedIndex = 0;
        }

        private void appComboBox_DropDownClosed(object sender, EventArgs e)
        {
            //string processName = (string)appComboBox.SelectedItem;
            //if (processName == null)
            //{
            //    processName = dummyProfile.Id; // get the previous profile id
            //    appComboBox.SelectedItem = processName;
            //}
            ////            if (!processName.Equals(DefaultApplicationName)) {
            //AppProfile profile;
            //if (profiles.TryGetValue(processName, out profile))
            //{
            //    dummyProfile = profile;
            //}
            //else
            //{
            //    dummyProfile = new AppProfile(processName);
            //}
            //            }

            resetGestureFields();
        }

        private void resetGestureFields()
        {
            //upTextBox.Text = selectedMap[Gestures.Up];
            //downTextBox.Text = selectedMap[Gestures.Down]; ;
            //leftTextBox.Text = selectedMap[Gestures.Left]; ;
            //rightTextBox.Text = selectedMap[Gestures.Right]; ;
        }

        private void appComboBox_DropDownOpened(object sender, EventArgs e)
        {
            Dictionary<String, String> processList = getTaskbarProcessNames();
            appComboBox.Items.Clear();
            appComboBox.Items.Add(DefaultApplicationName);

            foreach (string processName in processList.Keys)
            {
                appComboBox.Items.Add(processName);
            }
        }

        private Dictionary<String, String> getTaskbarProcessNames()
        {
            Process[] processList = Process.GetProcesses();
            Dictionary<String, String> result = new Dictionary<String, String>();

            foreach (Process process in processList)
            {
                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    String value;
                    if (!result.TryGetValue(process.ProcessName, out value))
                    {
                        result.Add(process.ProcessName, process.ProcessName);
                    }
                }
            }

            return result;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            resetGestureFields();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItem != DefaultApplicationName)
            {
                //dummyProfile.UpdateGestureMapping(Gestures.Up, upTextBox.Text);
                //dummyProfile.UpdateGestureMapping(Gestures.Down, downTextBox.Text);
                //dummyProfile.UpdateGestureMapping(Gestures.Left, leftTextBox.Text);
                //dummyProfile.UpdateGestureMapping(Gestures.Right, rightTextBox.Text);

                CreateDummyMap();
                MessageBox.Show("Saved");
            }    
        }

        private void appComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String item = (String)appComboBox.SelectedItem;
            
            if (!selectedItem.Equals(item))
            {
 //               selectedItem = item;
                //selectedMap = dummyMap;
                if (selectedItem != DefaultApplicationName)
                {
                    /*
                    Dictionary<String, String> map;                
                    if (GTSettings.Settings.Instance.Mappings.Map.TryGetValue(selectedItem, map))
                    {
                        selectedMap = map;
                    }
                */
                }                
            }            
        }

        private void CreateDummyMap()
        { 
        
        }






    }
}

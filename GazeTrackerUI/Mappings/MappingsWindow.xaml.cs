﻿using System;
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
using EyeSparkTrackingLibrary;
using GTSettings;

namespace GazeTrackerUI.Mappings
{
    /// <summary>
    /// Interaction logic for MappingsWindow.xaml
    /// </summary>
    public partial class MappingsWindow : Window
    {
        #region Imports
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd,
            StringBuilder buffer, int length);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        #endregion

        #region Fields
        private static string DefaultApplicationName = "-None Selected-";
        private const int MaxSequenceLength = 8;

        private static MappingsWindow instance;

        private String selectedItem = DefaultApplicationName;
        private DataItem defaultGesture;
        private Dictionary<String, String> selectedMap;
        private Dictionary<String, String> dummyMap;
        private KeySequence keySequence = new KeySequence();

        class DataItem
        {
            private object name;
            private object value;

            public DataItem(object n, object v)
            {
                name = n;
                value = v;
            }

            public Object Name
            {
                get { return name; }
            }

            public Object Value
            {
                get { return value; }
            }

            public override string ToString()
            {
                return name.ToString();
            }
        }

        #endregion

        #region Constructor
        private MappingsWindow()
        {
            CreateDummyMap();
            selectedMap = dummyMap;

            InitializeComponent();
        }
        #endregion

        #region Properties
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
        #endregion

        private void AppMinimize(object sender, MouseButtonEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            appComboBox.Items.Add(DefaultApplicationName);
            appComboBox.SelectedItem = DefaultApplicationName;

            defaultGesture = new DataItem("Pitch Up", Gesture.Pitch.Up);
            gesturesComboBox.Items.Add(defaultGesture);
            gesturesComboBox.Items.Add(new DataItem("Pitch Down", Gesture.Pitch.Down));
            gesturesComboBox.Items.Add(new DataItem("Roll Left", Gesture.Roll.Left));
            gesturesComboBox.Items.Add(new DataItem("Roll Right", Gesture.Roll.Right));
            gesturesComboBox.Items.Add(new DataItem("Yaw Left", Gesture.Yaw.Left));
            gesturesComboBox.Items.Add(new DataItem("Yaw Right", Gesture.Yaw.Right));

            gesturesComboBox.SelectedItem = defaultGesture;

            yawSlider.Value = Settings.Instance.HeadMovement.YawThreshold;
            pitchSlider.Value = Settings.Instance.HeadMovement.PitchThreshold;
            rollSlider.Value = Settings.Instance.HeadMovement.RollThreshold;

            Hardware.Instance.HeadMeasurement +=new HeadMeasurementEventHandler(Instance_HeadMeasurement);
            HeadTracker.Instance.HeadMovement +=new HeadMovementEventHandler(Instance_HeadMovement);
        }

        private void Instance_HeadMeasurement(object sender, HeadMeasurementEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                HeadTracker tracker = HeadTracker.Instance;
                this.yawOffsetLabel.Content = e.X - tracker.OriginX;
                this.pitchOffsetLabel.Content = e.Y - tracker.OriginY;
                this.rollOffsetLabel.Content = e.Z - tracker.OriginZ;
            }));

        }

        private void Instance_HeadMovement(object sender, HeadMovementEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                this.detectionTextBox.Text = e.Gesture;
            }));
            
        }

        private void appComboBox_DropDownClosed(object sender, EventArgs e)
        {
            resetFields();
        }

        private void resetFields()
        {
            appComboBox.SelectedItem = selectedItem;
            GetKeySequence();
            sequenceTextBox.Focus();
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
            resetFields();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItem != DefaultApplicationName)
            {
                DataItem item = (DataItem)gesturesComboBox.SelectedItem;
                selectedMap[(string)item.Value] = sequenceTextBox.Text;
                Settings.Instance.HeadMovement.SaveMapping(selectedItem, selectedMap);
                
                CreateDummyMap();

                MessageBox.Show("Saved");
            }
        }

        private void appComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String appName = (String)appComboBox.SelectedItem;

            if (appName != null && !selectedItem.Equals(appName))
            {
                selectedItem = appName;
                selectedMap = dummyMap;
                if (selectedItem != DefaultApplicationName)
                {
                    Dictionary<String, String> map =
                        Settings.Instance.HeadMovement.GetMapping(selectedItem);
                    if (map != null)
                    {
                        selectedMap = map;
                    }
                }
                Console.WriteLine("reseting gesture");
                if (gesturesComboBox.SelectedItem == defaultGesture)
                {
                    // force the fetching of a new sequence
                    Console.WriteLine("forcing new key sequence");

                    GetKeySequence();
                }
                else
                {
                    gesturesComboBox.SelectedItem = defaultGesture;
                }

                sequenceTextBox.Focus();
                //sequenceTextBox.SelectAll();
            }
        }

        private void GetKeySequence()
        {
            DataItem gesture = (DataItem)gesturesComboBox.SelectedItem;
            if (gesture != null)
            {
                Console.WriteLine("getting new key sequence");
                keySequence = new KeySequence(selectedMap[(string)gesture.Value]);
                sequenceTextBox.Text = keySequence.Text;
            }
        }

        private void CreateDummyMap()
        {
            dummyMap = new Dictionary<string, string>();
            dummyMap.Add(Gesture.Pitch.Up, "");
            dummyMap.Add(Gesture.Pitch.Down, "");
            dummyMap.Add(Gesture.Yaw.Left, "");
            dummyMap.Add(Gesture.Yaw.Right, "");
            dummyMap.Add(Gesture.Roll.Left, "");
            dummyMap.Add(Gesture.Roll.Right, "");

        }

        private void keyboardButton_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void keyboardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void keyboardButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // get processes
            Process[] pl = Process.GetProcesses();
            foreach (Process p in pl)
            {
                // find the process for the on screen keyboard
                if (p.ProcessName == "osk.exe")
                {
                    // if alive
                    if (p.MainWindowHandle != IntPtr.Zero)
                    {
                        // bring to front
                        SetForegroundWindow(p.MainWindowHandle);
                        return;
                    }
                }
            }

            // not found; start the process
            Process osk = new Process();
            osk.StartInfo.FileName = "osk.exe";
            osk.Start();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (keySequence.Length < MaxSequenceLength)
            {
                keySequence.Append(e.Key.ToString());
                sequenceTextBox.Text = keySequence.Text;

                if (keySequence.Length == MaxSequenceLength)
                {
                    sequenceTextBox.BorderBrush = Brushes.Red;
                }                
            }
            Console.WriteLine(keySequence.ToString());
        }

        private void gesturesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataItem gesture = (DataItem)gesturesComboBox.SelectedItem;
            GetKeySequence();
            sequenceTextBox.Focus();
        }

        private void clearLabel_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void clearLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void clearLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            keySequence.Clear();
            sequenceTextBox.Text = "";
            sequenceTextBox.BorderBrush = Brushes.WhiteSmoke;
        }

        private void DragWindow(object sender, MouseButtonEventArgs args)
        {
            try
            {
                DragMove();
            }
            catch (Exception)
            {
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sender.GetType() != typeof(GazeTrackerUIMainWindow))
            {
                e.Cancel = true;
                Hide();
            }
            

        }

        private void yawSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Visibility == Visibility.Visible)
            {
                Settings.Instance.HeadMovement.YawThreshold = (int)yawSlider.Value;
            }
            
        }

        private void rollSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Visibility == Visibility.Visible)
            {
                Settings.Instance.HeadMovement.RollThreshold = (int)rollSlider.Value;
            }
        }

        private void pitchSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Visibility == Visibility.Visible)
            {
                Settings.Instance.HeadMovement.PitchThreshold = (int)pitchSlider.Value;
            }
        }

        private void Recalibrate(object sender, RoutedEventArgs e)
        {
            HeadTracker.Instance.StartCalibration();  
        }



    }
}

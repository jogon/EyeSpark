using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace EyeSparkTrackingLibrary
{
    public class HeadTracker
    {
        #region Imports
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd,
            StringBuilder buffer, int length);
        #endregion

        #region Fields
        private static HeadTracker instance;
        #endregion

        #region Constructor
        private HeadTracker() { }
        #endregion

        #region Properties
        public static HeadTracker Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new HeadTracker();
                }
                return instance;
            }
        }
        #endregion

        #region Events
        public event HeadMovementEventHandler HeadMovement;
        #endregion

        public void Start()
        {
            System.Threading.Thread t = new System.Threading.Thread(StartThread);
            t.Start();
            //// do the tracking here

            ////            while (true) 
            ////            {
            //// we should get a reference to the device
            //// via the usb event.
            //if ("DEVICE" != null)
            //{
            //    // get window title
            //    IntPtr handle = GetForegroundWindow();
            //    int length = GetWindowTextLength(handle) + 1;
            //    StringBuilder title = new StringBuilder(length);
            //    GetWindowText(handle, title, title.Capacity);

            //    // get processes
            //    Process[] pl = Process.GetProcesses();
            //    foreach (Process p in pl)
            //    {
            //        // find the process for the active window
            //        if (p.MainWindowHandle == handle)
            //        {
            //            //AppProfile profile;
            //            //if (profiles.TryGetValue(p.ProcessName, out profile))
            //            //{
            //            //    string randGesture = gestures[random.Next(gestures.Length)];

            //            //    // TODO: fire off an event and let the main window handle it
            //            //    //SendKeys.SendWait(profile.GetGestureMapping(randGesture));

            //            //}
            //        }
            //    }
                //                }            
    //        }
        }

        private void StartThread()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(5000);
                OnHeadMovement(null);
            }
        }

        private void OnHeadMovement(HeadMovementEventArgs e) {
            if (HeadMovement != null)
            {
                HeadMovement(this, e);
            }
        }
    }
}

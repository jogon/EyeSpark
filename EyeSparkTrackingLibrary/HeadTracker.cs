using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace EyeSparkTrackingLibrary
{
    public class HeadTracker
    {
        #region Fields
        //Initialize HID Object with unique vendor_id and product_id.
        //Paremeters must be all lowercase for some shitty reason.
        static USBHIDDRIVER.USBInterface usb = 
            new USBHIDDRIVER.USBInterface("vid_03eb", "pid_204f");

        private static HeadTracker instance;
        private String[] dataMap = new String[6];

        private const byte PitchUp = 0;
        private const byte PitchDown = 1;
        private const byte RollLeft = 2;
        private const byte RollRight = 3;
        private const byte YawLeft = 4;
        private const byte YawRight = 5;

        #endregion

        #region Constructor
        
        private HeadTracker() {
            dataMap[PitchUp] = Gesture.Pitch.Up;
            dataMap[PitchDown] = Gesture.Pitch.Down;
            dataMap[RollLeft] = Gesture.Roll.Left;
            dataMap[RollRight] = Gesture.Roll.Right;
            dataMap[YawLeft] = Gesture.Yaw.Left;
            dataMap[YawRight] = Gesture.Yaw.Right;
        }
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

        public bool Stop
        {
            get;
            set;
        }

        #endregion

        #region Events
        public event HeadMovementEventHandler HeadMovement;
        #endregion

        public void Start()
        {
            Thread t = new Thread(StartThread);
            t.Name = "Head Movement";
            t.Start();
        }

        private void StartThread()
        {
            usb.enableUsbBufferEvent(new System.EventHandler(OnHeadMovement));
                        
            while (!Stop)
            {
                // TODO: [EyeSpark] follow event based model 
                // instead of this.
                while (!usb.Connect())
                {
                    Thread.Sleep(1000);
                }
                Console.WriteLine("HeadTracker: Connected to USB");
                usb.startRead();
                while (true)
                {
                    if (!usb.Connect())
                    {
                        Console.WriteLine("HeadTracker: Connected to USB siezed");                        
                        usb.stopRead(); // do I need to call this?
                        break;
                    }
                    Thread.Sleep(1000);
                }
            }
        }

        private void OnHeadMovement(object sender, EventArgs e) {
            if (HeadMovement != null)
            {
                //////////////////////////////////////////////////////////////////////////////////////////////////
                // Don't edit anything from HERE...
                //////////////////////////////////////////////////////////////////////////////////////////////////
                if (USBHIDDRIVER.USBInterface.usbBuffer.Count > 0)
                {
                    byte[] currentRecord = null;
                    int counter = 0;
                    while ((byte[])USBHIDDRIVER.USBInterface.usbBuffer[counter] == null)
                    {
                        //Remove this report from list
                        lock (USBHIDDRIVER.USBInterface.usbBuffer.SyncRoot)
                        {
                            USBHIDDRIVER.USBInterface.usbBuffer.RemoveAt(0);
                        }
                    }
                    //since the remove statement at the end of the loop take the first element
                    currentRecord = (byte[])USBHIDDRIVER.USBInterface.usbBuffer[0];
                    lock (USBHIDDRIVER.USBInterface.usbBuffer.SyncRoot)
                    {
                        USBHIDDRIVER.USBInterface.usbBuffer.RemoveAt(0);
                    }

                    //////////////////////////////////////////////////////////////////////////////////////////////////
                    // ... to HERE.
                    //////////////////////////////////////////////////////////////////////////////////////////////////


                    //////////////// Do stuff with record /////////////////////////
                    //Console.WriteLine("Record has [" + currentRecord.Length + "] bytes");

                    // Skip the first byte on purpose; it has special meaning

                    short data;

                    data = 0;
                    data += currentRecord[1];
                    data += (short)(currentRecord[2] << 8);
                    Console.WriteLine("x: " + data);

                    data = 0;
                    data += currentRecord[3];
                    data += (short)(currentRecord[4] << 8);
                    Console.WriteLine("y: " + data);

                    data = 0;
                    data += currentRecord[5];
                    data += (short)(currentRecord[6] << 8);
                    Console.WriteLine("z: " + data);
                    
                        //if (currentRecord[1] >= 0 && currentRecord[1] < dataMap.Length)
                        //{
                        //    HeadMovement(this,
                        //        new HeadMovementEventArgs(dataMap[currentRecord[1]]));
                        //}
                    ////////////////// Done with record ///////////////////////////
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace EyeSparkTrackingLibrary
{
    public class Hardware
    {
        private static Hardware instance;
        //Initialize HID Object with unique vendor_id and product_id.
        //Paremeters must be all lowercase for some shitty reason.
        private static USBHIDDRIVER.USBInterface usb =
            new USBHIDDRIVER.USBInterface("vid_03eb", "pid_204f");

        #region Events

        public event HeadMeasurementEventHandler HeadMeasurement;
        
        #endregion

        #region Properties
        public static Hardware Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Hardware();
                }
                return instance;
            }
        }

        public bool Connected
        {
            get {
                return usb.Connect();
            }
        }
        #endregion

        public bool StartCommunication() 
        {
            if (Connected)
            {
                usb.enableUsbBufferEvent(
                    new System.EventHandler(OnDataAvailable));

                usb.startRead();
                return true;
            }
            return false;
        }

        public bool StopCommunication()
        {
            if (Connected)
            {
                usb.stopRead();
                return true;
            }
            return false;
        }

        private void OnDataAvailable(object sender, EventArgs e)
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

                if (HeadMeasurement != null)
                {
                    Int16 x;
                    Int16 y;
                    Int16 z;

                    x = 0;
                    x += currentRecord[1];
                    x += (Int16)(currentRecord[2] << 8);
                    //Console.WriteLine("x: " + x);

                    y = 0;
                    y += currentRecord[3];
                    y += (Int16)(currentRecord[4] << 8);
                    //Console.WriteLine("y: " + y);

                    z = 0;
                    z += currentRecord[5];
                    z += (Int16)(currentRecord[6] << 8);
                    //Console.WriteLine("z: " + z);

                    HeadMeasurement(this, new HeadMeasurementEventArgs(x, y, z));
                }
                
                ////////////////// Done with record ///////////////////////////

            }
        }

        private Hardware()
        {
            // Try and connect on init.
            usb.Connect();
        }
    }
}

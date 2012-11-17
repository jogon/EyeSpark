using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using USBHIDDRIVER.USB;
using USBHIDDRIVER.List;

namespace HIDTest
{
    class Program
    {

        //Initialize HID Object with unique vendor_id and product_id.
        //Paremeters must be all lowercase for some shitty reason.
        static USBHIDDRIVER.USBInterface usb = new USBHIDDRIVER.USBInterface("vid_03eb", "pid_204f");

        static void Main(string[] args)
        {

            // Get a list of devices that match vid and pid
            // This list should NEVER have more than one element.
            Console.WriteLine("USB HID Test");
            String[] list = usb.getDeviceList();
            Console.WriteLine("Devices with vid = 0x046d and pid = 0xc245: " + list.Length);

            // usb.Connect() returns false if the device is not physically connected.
            while (!usb.Connect())
            {
                Thread.Sleep(1000);
                Console.WriteLine("Not Connected");
            }

            
            Console.WriteLine("Connected!");


            // This only attaches "myEventCatcher" to the UsbBufferEvent.  
            Console.WriteLine("Enabling Buffer Event...");
            usb.enableUsbBufferEvent(new System.EventHandler(myEventCatcher));


            // After calling usb.startRead(), "myEventCacher" will be called whenever the device sends data to the PC.
            Console.WriteLine("Press a key to begin data dump.");
            Console.ReadKey();
            usb.startRead();


            // Device is configured and reading, you can keep on doing your important shit.
            while (true)
            {
                
                if (!usb.Connect()) {
                    Console.WriteLine("Device Disconnected");
                }

                Thread.Sleep(1000);
            }

        }



        // Gets data from device.
        private static void myEventCatcher(object sender, EventArgs e)
        {
            //Debug
            Console.WriteLine("=============== EVENT HAS BEEN CAUGHT ===============\n");
            Console.WriteLine("USB Buffer Contains: " + USBHIDDRIVER.USBInterface.usbBuffer.Count);


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
                Console.WriteLine("Record has [" + currentRecord.Length + "] bytes");

                for (int i = 1; i < currentRecord.Length; i++)
                {
                    Console.WriteLine("Byte[" + i + "] =" + currentRecord[i]);
                }

                ////////////////// Done with record ///////////////////////////
            }

            //Debug
            Console.WriteLine("=============== END EVENT ===============");

        }


    }
}
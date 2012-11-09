using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBHIDDRIVER.USB;
using USBHIDDRIVER.List;
using System.Threading;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("USB HID Test");

            USBHIDDRIVER.USBInterface usb = new USBHIDDRIVER.USBInterface("vid_046d","pid_c245");

            String[] list = usb.getDeviceList();
            Console.WriteLine("The amount of devices with vid = 0x046d and pid = 0xc245: " + list.Length);

            usb.Connect();

            usb.enableUsbBufferEvent(new System.EventHandler(myEventCacher));

            Thread.Sleep(5);

            usb.startRead();

            //...

            usb.stopRead();

            usb.stopRead();
            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        void myEventCacher()
        {
            Console.WriteLine("STUFF HAPPENED");
        }
    }
}

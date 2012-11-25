using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EyeSparkTrackingLibrary
{
    public delegate void HeadMovementEventHandler(object sender,
       HeadMovementEventArgs e);

    public class HeadMovementEventArgs : EventArgs
    {
        private string gesture;

        public String Gesture
        {
            get 
            {
                return gesture;
            }
        }
        public HeadMovementEventArgs(String gesture)
        {
            this.gesture = gesture;
        }
    }
}

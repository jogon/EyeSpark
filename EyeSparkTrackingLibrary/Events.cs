using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EyeSparkTrackingLibrary
{
    public delegate void HeadMovementEventHandler(object sender,
       HeadMovementEventArgs e);

    public delegate void HeadMeasurementEventHandler(object sender,
       HeadMeasurementEventArgs e);


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

    public class HeadMeasurementEventArgs : EventArgs
    {
        #region Fields
        #endregion

        #region Properties

        public Int16 X { get; set; }

        public Int16 Y { get; set; }

        public Int16 Z { get; set; }

        #endregion

        #region Constructor

        public HeadMeasurementEventArgs(Int16 x, Int16 y, Int16 z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        #endregion

        
    }
}

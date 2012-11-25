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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using GTSettings;
using System.ComponentModel;

namespace EyeSparkTrackingLibrary
{
    public class HeadTracker
    {
        #region Fields

        private const Int16 MaxCalibrationCount = 10;
        private const int YawIndex = 0;
        private const int PitchIndex = 1;
        private const int RollIndex = 2;

        private static HeadTracker instance;
        
        private String[] gestures;
        private int[] thresholds;
        private int calibrationCount = 0;
        private Int16 originX = 0;
        private Int16 originY = 0;
        private Int16 originZ = 0;


        #endregion

        #region Events

        public event HeadMovementEventHandler HeadMovement;

        #endregion

        #region Constructor

        private HeadTracker()
        {
            DoMeasure = true;
            gestures = new String[]{
                    Gesture.Yaw.Right,
                    Gesture.Yaw.Left,
                    Gesture.Pitch.Down,
                    Gesture.Pitch.Up,
                    Gesture.Roll.Right,
                    Gesture.Roll.Left
                };

            //gestures = new String[]{
            //        Gesture.Roll.Right,
            //        Gesture.Roll.Left,
            //        Gesture.Yaw.Left,
            //        Gesture.Yaw.Right,
            //        Gesture.Pitch.Up,
            //        Gesture.Pitch.Down
            //    };

            thresholds = new int[3];
            thresholds[YawIndex] = Settings.Instance.HeadMovement.YawThreshold;
            thresholds[PitchIndex] = Settings.Instance.HeadMovement.PitchThreshold;
            thresholds[RollIndex] = Settings.Instance.HeadMovement.RollThreshold;

            Hardware.Instance.HeadMeasurement += OnHeadMeasurement;
            
            Settings.Instance.HeadMovement.PropertyChanged+=
                new System.ComponentModel.PropertyChangedEventHandler(
                    HeadMovement_PropertyChanged);

            Calibrating = true;
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

        private bool Calibrating { get; set; }

        public bool DoMeasure { get; set; }

        public Int16 OriginX { get { return originX; } }
        public Int16 OriginY { get { return originY; } }
        public Int16 OriginZ { get { return originZ; } }

        #endregion

        public bool Start()
        {
            //if (Hardware.Instance.Connected)
            //{ 
            //    Hardware.Instance.HeadMeasurement += OnHeadMeasurement;
            //    Hardware.Instance.StartCommunication();
            //    return true;
            //}
            //return false;
            
            //Calibrate();

            return Hardware.Instance.StartCommunication();
        }

        public void Stop()
        {
            Hardware.Instance.StopCommunication();
        }

        public bool ConnectToDevice()
        {
            return Hardware.Instance.Connected;
        }

        public void StartCalibration()
        {
            Console.WriteLine("Begin Head Tracker Calibration");
            Calibrating = true;
            originX = 0;
            originY = 0;
            originZ = 0;
            calibrationCount = 0;

            // problematic if thread not started previously
        }

        public void StopCalibration() 
        {
            Calibrating = false;
            originX = (Int16)(originX / calibrationCount);
            originY = (Int16)(originY / calibrationCount);
            originZ = (Int16)(originZ / calibrationCount);

            Console.WriteLine("Finished Calibration. x:{0} y :{1} z:{2}",
                        originX, originY, originZ);
        }

        public void HeadMovement_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            String propertyName = e.PropertyName;

            if (propertyName.Equals("yawThreshold"))
            {
                
                thresholds[YawIndex] =
                    Settings.Instance.HeadMovement.YawThreshold;
                Console.WriteLine("[HeadTraker.cs] Set {0} to {1}",
                    propertyName, thresholds[YawIndex]);
            }
            else if (propertyName.Equals("pitchThreshold"))
            {
                thresholds[PitchIndex] =
                    Settings.Instance.HeadMovement.PitchThreshold;
                Console.WriteLine("[HeadTraker.cs] Set {0} to {1}",
                    propertyName, thresholds[PitchIndex]);
            }
            else if (propertyName.Equals("rollThreshold"))
            {
                thresholds[RollIndex] =
                    Settings.Instance.HeadMovement.RollThreshold;
                Console.WriteLine("[HeadTraker.cs] Set {0} to {1}",
                    propertyName, thresholds[RollIndex]);
            }

        }

        private void OnHeadMeasurement(object sender, HeadMeasurementEventArgs e)
        {
            /*
             * Process head measurements here
             * 
             * */
            Int16 newX = e.X; // yaw
            Int16 newY = e.Y; // pitch
            Int16 newZ = e.Z; // roll
            

            if (Calibrating)
            {
                originX += newX;
                originY += newY;
                originZ += newZ;
                calibrationCount++;
                //originX = Math.Abs(newX - originX) > 5 ? newX : originX;
                //originY = Math.Abs(newY - originY) > 5 ? newY : originY;
                //originZ = Math.Abs(newZ - originZ) > 5 ? newZ : originZ;
                ////Console.WriteLine("oz: " + originZ);

                if (calibrationCount == MaxCalibrationCount)
                {
                    StopCalibration();
                }
            }
            else
            {
                Int16[] diff = new Int16[3];
                diff[0] = (Int16)(newX - originX);
                diff[1] = (Int16)(newY - originY);
                diff[2] = (Int16)(newZ - originZ);

                if (DoMeasure)
                {
                    int max = FindMaxMagnitude(diff);
                    if (Math.Abs(diff[max]) > thresholds[max])
                    {                        
                        DoMeasure = false;
                        int index;
                        if (diff[max] > 0)
                        {
                            index = 2 * max;
                        }
                        else
                        {
                            index = 2 * max + 1;

                        }
                        
                        Console.WriteLine("[{0}] Gesture detected: {1}. Moved {2} from origin.",
                            Thread.CurrentThread.GetHashCode(), gestures[index], diff[max]);

                        OnHeadMovement(new HeadMovementEventArgs(gestures[index],newX,newY,newZ));
                    }
                }
                else
                {
                    int i = 0;
                    for (; i < diff.Length; i++)
                    {
                        if (Math.Abs(diff[i]) > thresholds[i])
                        {
                            break;
                        }
                    }
                    if (i == diff.Length)
                    {
                        DoMeasure = true;
                    }
                }
            }
        }

        private void OnHeadMovement(HeadMovementEventArgs e)
        {
            if (HeadMovement != null)
            {
                HeadMovement(this, e);
            }
        }

        private int FindMaxMagnitude(Int16[] a)
        {
            int current = 0;
            for (int i = 1; i < a.Length; i++)
            {
                if (Math.Abs(a[i]) > Math.Abs(a[current]))
                {
                    current = i;
                }
            }
            return current;
        }
    }
}


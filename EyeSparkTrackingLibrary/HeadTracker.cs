using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Timers;

namespace EyeSparkTrackingLibrary
{
    public class HeadTracker
    {
        #region Fields

        private const Int16 MaxCalibrationSteps = 150;
        private const Int16 DefaultThreshold = 10;
        private static HeadTracker instance;
        //private String[] dataMap = new String[6];

        //private const byte PitchUp = 0;
        //private const byte PitchDown = 1;
        //private const byte RollLeft = 2;
        //private const byte RollRight = 3;
        //private const byte YawLeft = 4;
        //private const byte YawRight = 5;

        private String[] gestures;
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
                    Gesture.Yaw.Left,
                    Gesture.Yaw.Right,
                    Gesture.Pitch.Up,
                    Gesture.Pitch.Down,
                    Gesture.Roll.Right,
                    Gesture.Roll.Left
                };
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

        public bool Calibrating { get; set; }

        public bool DoMeasure { get; set; }

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
            Hardware.Instance.HeadMeasurement += OnHeadMeasurement;
            //Calibrate();

            return Hardware.Instance.StartCommunication();
        }

        public void Stop()
        {
            Hardware.Instance.StopCommunication();
        }

        public void Calibrate()
        {
            Console.WriteLine("Begin Head Tracker Calibration");
            Calibrating = true;
            originX = 0;
            originY = 0;
            originZ = 0;
            calibrationCount = 0;
        }

        private void OnHeadMeasurement(object sender, HeadMeasurementEventArgs e)
        {
            /*
             * Process head measurements here
             * 
             * */
            Int16 newX = e.X;
            Int16 newY = e.Y;
            Int16 newZ = e.Z;
            

            if (Calibrating)
            {
                calibrationCount++;
                originX = Math.Abs(newX - originX) > 5 ? newX : originX;
                originY = Math.Abs(newY - originY) > 5 ? newY : originY;
                originZ = Math.Abs(newZ - originZ) > 5 ? newZ : originZ;
                //Console.WriteLine("oz: " + originZ);

                if (calibrationCount == MaxCalibrationSteps)
                {
                    Console.WriteLine("Finished Calibration. x:{0} y :{1} z:{2}",
                        originX, originY, originZ);
                    Calibrating = false;
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
                    if (Math.Abs(diff[max]) > DefaultThreshold)
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
                        //eventQueue.Enqueue(new HeadMovementEventArgs(gestures[index]));
                        Console.WriteLine("[{0}] Gesture detected: {1}.",
                            Thread.CurrentThread.GetHashCode(), gestures[index]);
                    }
                }
                else
                {
                    int i = 0;
                    for (; i < diff.Length; i++)
                    {
                        if (Math.Abs(diff[i]) > DefaultThreshold)
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


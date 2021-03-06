using System;
using Microsoft.Kinect;


namespace SandWorm
{
    static class KinectController
    {
        public static KinectSensor sensor = null;
        public static int depthHeight = 0;
        public static int depthWidth = 0;
        public static MultiSourceFrameReader multiFrameReader = null;
        public static FrameDescription depthFrameDescription = null;
        public static int refc = 0;
        public static ushort[] depthFrameData = null;

        public static void AddRef()
        {
            if (sensor == null)
            {
                Initialize();
            }
            if (sensor != null)
            {
                refc++;
            }
        }

        public static void RemoveRef()
        {
            refc--;
            if ((sensor != null) && (refc == 0))
            {
                multiFrameReader.MultiSourceFrameArrived -= Reader_FrameArrived;
                sensor.Close();
                sensor = null;
            }
        }

        public static void Initialize()
        {
            sensor = KinectSensor.GetDefault();


            multiFrameReader = sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Depth);
            multiFrameReader.MultiSourceFrameArrived += new EventHandler<MultiSourceFrameArrivedEventArgs>(KinectController.Reader_FrameArrived);

            sensor.Open();
        }

        private static void Reader_FrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            if (e.FrameReference != null)
            {
                MultiSourceFrame multiFrame = e.FrameReference.AcquireFrame();
                if (multiFrame.DepthFrameReference != null)
                {
                    try
                    {
                        using (DepthFrame depthFrame = multiFrame.DepthFrameReference.AcquireFrame())

                        {
                            if (depthFrame != null)
                            {
                                using (KinectBuffer buffer = depthFrame.LockImageBuffer())
                                {
                                    depthFrameDescription = depthFrame.FrameDescription;
                                    depthWidth = depthFrameDescription.Width;
                                    depthHeight = depthFrameDescription.Height;
                                    depthFrameData = new ushort[depthWidth * depthHeight];
                                    depthFrame.CopyFrameDataToArray(depthFrameData);
                                }
                            }
                        }
                    }
                    catch (Exception) { return; }
                }
            }
        }

        public static KinectSensor Sensor
        {
            get
            {
                if (sensor == null)
                {
                    Initialize();
                }
                return sensor;
            }
            set
            {
                sensor = value;
            }
        }
    }
}
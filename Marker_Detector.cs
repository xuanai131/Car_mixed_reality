namespace OpenCvSharp.Demo
{
    using NativeWebSocket;
    using UnityEngine;
    using System.Collections;
    using UnityEngine.UI;
    using Aruco;
    using System.Collections.Generic;
    using System.IO;

    public class Marker_Detector : MonoBehaviour
    {
        public Texture2D texture;
        RawImage left;
        RawImage right;
        int sbb = 10;
        string info1, info21, info22, info31, info32;

        void Detect(Texture2D tex)
        {
            // Create default parameres for detection
            DetectorParameters detectorParameters = DetectorParameters.Create();

            // Dictionary holds set of all available markers
            Dictionary dictionary = CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.Dict6X6_250);

            // Variables to hold results
            Point2f[][] corners;
            int[] ids;
            Point2f[][] rejectedImgPoints;

            // Create Opencv image from unity texture
            Mat mat = Unity.TextureToMat(tex);

            // Convert image to grasyscale
            Mat grayMat = new Mat();
            Cv2.CvtColor(mat, grayMat, ColorConversionCodes.BGR2GRAY);
            int width = mat.Width;
            int height = mat.Height;

            // Detect and draw markers
            CvAruco.DetectMarkers(grayMat, dictionary, out corners, out ids, detectorParameters, out rejectedImgPoints);
            List<Point2f> pts2 = new List<Point2f>();
            pts2.Add(new OpenCvSharp.Point(0, 0));
            pts2.Add(new OpenCvSharp.Point(height, 0));
            pts2.Add(new OpenCvSharp.Point(0, width));
            pts2.Add(new OpenCvSharp.Point(height, width));
            OpenCvSharp.Size q = new OpenCvSharp.Size(width, height);
            Mat warp_blank = new Mat();
            Mat blank = new Mat();
            for (int i = 0; i < ids.Length; i++)
            {
                List<Point2f> pts1 = new List<Point2f>();
                pts1.Add(new OpenCvSharp.Point(corners[i][0].X, corners[i][0].Y));
                pts1.Add(new OpenCvSharp.Point(corners[i][1].X, corners[i][1].Y));
                pts1.Add(new OpenCvSharp.Point(corners[i][3].X, corners[i][3].Y));
                pts1.Add(new OpenCvSharp.Point(corners[i][2].X, corners[i][2].Y));
                blank = new Mat(2000, 2000, MatType.CV_8UC3, 3);

                TextAsset puzdata = (TextAsset)Resources.Load("traffic_sign", typeof(TextAsset));
                StringReader reader = new StringReader(puzdata.text);

                for (int k = 1; k <= 5 * sbb; k++)
                {
                    string line = reader.ReadLine();
                    if (ids[i] * 5 + 1 == k) { info1 = line; }
                    if (ids[i] * 5 + 2 == k) { info21 = line; }
                    if (ids[i] * 5 + 3 == k) { info22 = line; }
                    if (ids[i] * 5 + 4 == k) { info31 = line; }
                    if (ids[i] * 5 + 5 == k) { info32 = line; }
                }
                int x = -200;
                Cv2.PutText(blank, info1, new Point(corners[i][0].X + x, corners[i][0].Y),
                    HersheyFonts.HersheySimplex, 6, new Scalar(0, 0, 255), 12);
                Cv2.PutText(blank, info21, new Point(corners[i][0].X + x, corners[i][0].Y + 150),
                    HersheyFonts.HersheySimplex, 6, new Scalar(0, 0, 255), 12);
                Cv2.PutText(blank, info22, new Point(corners[i][0].X + x, corners[i][0].Y + 300),
                    HersheyFonts.HersheySimplex, 6, new Scalar(0, 0, 255), 12);
                Cv2.PutText(blank, info31, new Point(corners[i][0].X + x, corners[i][0].Y + 550),
                    HersheyFonts.HersheySimplex, 6, new Scalar(0, 0, 255), 12);
                Cv2.PutText(blank, info32, new Point(corners[i][0].X + x, corners[i][0].Y + 700),
                    HersheyFonts.HersheySimplex, 6, new Scalar(0, 0, 255), 12);

                Mat M = Cv2.GetPerspectiveTransform(pts2, pts1);
                //Mat warp_blank = new Mat();
                Cv2.WarpPerspective(blank, warp_blank, M, q, InterpolationFlags.Linear, BorderTypes.Constant);
                Mat graymask = new Mat();
                Cv2.CvtColor(warp_blank, graymask, ColorConversionCodes.BGR2GRAY);
                Mat mask = graymask.Threshold(10, 255, ThresholdTypes.Binary);
                Mat mask_inv = new Mat();
                Cv2.BitwiseNot(mask, mask_inv);
                Cv2.BitwiseAnd(mat, mask_inv.CvtColor(ColorConversionCodes.GRAY2BGR), mat);
                mat += warp_blank;
            }

            //CvAruco.DrawDetectedMarkers(mat, corners, ids);

            // Create Unity output texture with detected markers
            Texture2D outputTexture = Unity.MatToTexture(mat);

            // Set texture to see the result
            left.texture = outputTexture;
            right.texture = outputTexture;
            Resources.UnloadUnusedAssets();
        }

        WebSocket webSocket;
        // Start is called before the first frame update
        async void Start()
        {
            left = transform.parent.Find("Left").GetComponent<RawImage>();
            right = transform.parent.Find("Right").GetComponent<RawImage>();
            //Detect(texture);
            webSocket = new WebSocket("ws://192.168.191.103:8888");
            webSocket.OnOpen += () => { print("Connection Open!"); };
            webSocket.OnError += (e) => { print("Error :" + e); };
            webSocket.OnClose += (e) => { print("Connection Close!"); };
            webSocket.OnMessage += (bytes) =>
            {
                //print("onMessage length :" + bytes.Length);
                if (bytes.Length > 0)
                {
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(bytes);
                    Detect(tex);
                }
            };
            await webSocket.Connect();
        }

        // Update is called once per frame
        void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            webSocket.DispatchMessageQueue();
#endif
        }

        private async void OnApplicationQuit()
        {
            await webSocket.Close();
        }
    }
}
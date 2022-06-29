using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;
using Assets.LSL4Unity.Scripts;
using Valve.Newtonsoft.Json;
using System.Linq;
using System;
using Valve.Newtonsoft.Json.Linq;

/*
VERSION 2.1.1
Re-enabled hard-coded LSL integration from Semantic Comprehension version
VERSION 2.2.0
Neuroscan integration including dictionary for int->string marker conversion
*/

/*
 * Author: Alexander Minton
 * CIBCI Lab, University of Technology Sydney (UTS)
 * Sydney, 29th June, 2022
 * Email: Alex.Minton@hotmail.com
 */

public class Logger : MonoBehaviour
{
    static public string logFileName;
    static public LogWrapper log = new LogWrapper();
    static private LSLMarkerStream markerStream;
    static public JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
    static public Logger instance;

    [Tooltip("Enable numeric markers from 0-255 and store relevant lookup data in the log file")]
    public bool useNeuroscan;

    [Tooltip("Frequency of the aquisition of camera data")]
    public float camSnapshotFrequency;
    public bool logCamera;

    [Tooltip("Write log in real time instead of on application quit")]
    public bool realTimeLogging;

    [Tooltip("Frequency of mouse cursor location logging")]
    public float mouseSnapshotFrequency;
    public bool logMousePosition;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        logFileName = Application.persistentDataPath + "/Experiment-" + System.DateTime.Now.ToShortDateString().Replace('/', '-') + "-" + System.DateTime.Now.ToShortTimeString().Replace(':', '-') + ".json";
       
        markerStream = FindObjectOfType<LSLMarkerStream>();
        markerStream.gameObject.active = false;
        markerStream.gameObject.active = true;

        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

       // OldLogToNew("Experiment-11-10-2020-3-12 PM non stress.json");
        //OldLogToNew("Experiment-11-10-2020-3-28 PM stress.json");
       // OldLogToNew("Experiment-11-10-2020-3-42 PM stress 2.json");
    }

    // Update is called once per frame
    void Update()
    {
        if (log.needToWrite() && realTimeLogging) log.LogToFile(logFileName);
        if ((Time.time >= CameraSnapshot.lastSnapshot + 1/camSnapshotFrequency) && logCamera)
        {
            Logger.log.cameraSnapshots.Add(new CameraSnapshot());
            CameraSnapshot.lastSnapshot = Time.time;
        }
        if ((Time.time >= MouseTracking.MouseSnapshot.lastSnapshot + 1/mouseSnapshotFrequency) && logMousePosition)
        {
            Logger.log.mouseTracking.TakeMouseSnapshot();
        }
    }

    private void OnApplicationQuit()
    {
        log.LogToFile(logFileName);
    }

    #region Classes
    [System.Serializable]
    public class TrialWrapper
    {
        public string classname;
        public object trial;
        //public string serializedObject;
        public TrialWrapper(object inTrial)
        {
            trial = inTrial;
           //serializedObject = Valve.Newtonsoft.Json.JsonConvert.SerializeObject(trial);
             if (inTrial != null) classname = inTrial.GetType().FullName;
        }
    }
    [System.Serializable]
    public class LogWrapper
    {
        public List<TrialWrapper> trials;
        //public List<EyeTrackingLogger.EyeGazeData> eyeGazeData;
        public List<CameraSnapshot> cameraSnapshots;
        public MouseTracking mouseTracking;
        public Dictionary<string, int> neuroscanLookupTable;
        public LogWrapper()
        {
            trials = new List<TrialWrapper>();
            //eyeGazeData = new List<EyeTrackingLogger.EyeGazeData>();
            cameraSnapshots = new List<CameraSnapshot>();
            mouseTracking = new MouseTracking();
            neuroscanLookupTable = new Dictionary<string, int>();
        }
        private bool writeflag = false;
        public void LogToFile(string filename)
        {
            //string thisJson = JsonUtility.ToJson(this);
            string thisJson = Valve.Newtonsoft.Json.JsonConvert.SerializeObject(this, Logger.serializerSettings);
            System.IO.File.WriteAllText(filename, thisJson);// + "\n");
            writeflag = false;
        }
        public void AddToLog(object inTrial)
        {
                trials.Add(new TrialWrapper(inTrial));
            writeflag = true;
        }
        public bool needToWrite()
        {
            return writeflag;
        }
    }

    [System.Serializable]
    public class Condition
    {
        public string conditionName;
        public float startTime;
        public float endTime;
        public DateTime realStart;
    }

    [System.Serializable]
    public class CameraSnapshot
    {
       public static float lastSnapshot = 0;
        public Vector3 pos;
        public Vector3 rot;
       // public float[] pos;
       // public float[] rot;
        public float time;

        public CameraSnapshot()
        {
            Transform cam = Camera.main.transform;
            pos = cam.position;
            rot = cam.rotation.eulerAngles;
            //pos = new float[3]{ cam.position.x, cam.position.y, cam.position.z};
            //rot = new float[3]{ cam.rotation.eulerAngles.x, cam.rotation.eulerAngles.y, cam.rotation.eulerAngles.z };
            time = Time.time;
        }
    }

    [System.Serializable]
    public class MouseTracking
    {
        public List<MouseSnapshot> mouseSnapshots;
        public List<MouseTrackedObject> mouseTrackedObjects;

        [System.Serializable]
        public class MouseTrackedObject
        {
            public string name;
            Rect screenAdjustedPixelPosition;
            Vector3[] corners;
            public float x;
            public float y;
            public float height;
            public float width;
            public MouseTrackedObject(GameObject gameObject)
            {
                RectTransform objRect = gameObject.GetComponent<RectTransform>();
                Canvas screenCanvas = gameObject.GetComponentInParent<Canvas>();
                screenAdjustedPixelPosition = RectTransformUtility.PixelAdjustRect(objRect, screenCanvas);
                corners = new Vector3[4];
                objRect.GetWorldCorners(corners);
                x = corners[0].x;
                y = corners[0].y;
                height = screenAdjustedPixelPosition.height;
                width = screenAdjustedPixelPosition.width;
                //x = screenAdjustedPosition.x;
                //y = screenAdjustedPosition.y;
                //height = screenAdjustedPosition.height;
                //width = screenAdjustedPosition.width;
                name = gameObject.name;
            }
        }

        [System.Serializable]
        public class MouseSnapshot
        {
            public static float lastSnapshot = 0;
            // public Vector3 pos; //Removed due to json bloat
            public float x;
            public float y;
            public float time;
            public MouseSnapshot()
            {
                //pos = Input.mousePosition;
                x = Input.mousePosition.x;
                y = Input.mousePosition.y;
                time = Time.time;
            }
        }

        public void AddMouseTrackedCanvasObject(GameObject gameObject)
        {
            mouseTrackedObjects.Add(new MouseTrackedObject(gameObject));
        }

        public void TakeMouseSnapshot()
        {
            mouseSnapshots.Add(new MouseSnapshot());
            MouseSnapshot.lastSnapshot = Time.time;
        }

        public MouseTracking()
        {
            mouseSnapshots = new List<MouseSnapshot>();
            mouseTrackedObjects = new List<MouseTrackedObject>();
        }
    }
    #endregion

    void OldLogToNew(string filename)
    {
        string filepath = Application.persistentDataPath + "/"+filename;
        string fileJson = System.IO.File.ReadAllText(filepath);
        LogWrapper oldLog = Valve.Newtonsoft.Json.JsonConvert.DeserializeObject<LogWrapper>(fileJson, Logger.serializerSettings);
        List<TrialWrapper> delList = new List<TrialWrapper>();
        LogWrapper newLog = new LogWrapper();
        JsonSerializer serializer = new JsonSerializer();
        foreach (TrialWrapper wrapper in oldLog.trials)
        {
            switch (wrapper.classname)
            {
                case "EyeTrackingLogger+EyeGazeData":
                    {
                        //EyeTrackingLogger.EyeGazeData data = (EyeTrackingLogger.EyeGazeData)serializer.Deserialize(new JTokenReader((JToken)wrapper.trial), typeof(EyeTrackingLogger.EyeGazeData));
                        //newLog.eyeGazeData.Add((EyeTrackingLogger.EyeGazeData)data);
                        break;
                    }
                case "Logger+CameraSnapshot":
                    {
                        CameraSnapshot data = (CameraSnapshot)serializer.Deserialize(new JTokenReader((JToken)wrapper.trial), typeof(CameraSnapshot));
                        newLog.cameraSnapshots.Add((CameraSnapshot)data);
                        break;
                    }
                default:
                    {
                        newLog.trials.Add(wrapper);
                        break;
                    }
            }
        }
        string newJson = Valve.Newtonsoft.Json.JsonConvert.SerializeObject(newLog, Logger.serializerSettings);
        string newFileName = filename.TrimEnd(".json".ToCharArray()) + "-newFormat.json";
        string newFilePath =  Application.persistentDataPath + "/" + newFileName;
        System.IO.File.WriteAllText(newFilePath, newJson);

    }

    static public void SendMarker(string inMarker)
    {
        if (!Logger.instance.useNeuroscan) markerStream.Write(inMarker);
        else //Check if dict contains marker, if yes, send relevant key int, if no, add to dict, then send new key int.
        {
            int lookupNum = -1;
            bool successfulLookup = log.neuroscanLookupTable.TryGetValue(inMarker, out lookupNum);
            if (successfulLookup) markerStream.Write(lookupNum.ToString());
            else //marker not in dict, add then send new value
            {
                //get largest value in the dict;
                int nextMarker = log.neuroscanLookupTable.Count(); //If count is 0, default to 0, if count is 10 (index 0-9) the next free marker is 10
                if (nextMarker > 255) throw new Exception("Neuroscan Lookup Table out of range (Maximum range 0-255)"); //brick if we try to go above 255
                else
                {
                    log.neuroscanLookupTable.Add(inMarker, nextMarker); UdpSender.SendToNeuroscan(nextMarker.ToString());
                }
            }
        }
    }
}


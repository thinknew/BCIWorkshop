using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Author: Alexander Minton
 * CIBCI Lab, University of Technology Sydney (UTS)
 * Sydney, 29th June, 2022
 * Email: Alex.Minton@hotmail.com
 */

public class SSVEPController : MonoBehaviour
{
    static public SSVEPController instance;
    bool firstTimeSetupRun = false;

    public List<GameObject> SSVEPSquares;
    public GameObject SSVEPSquare;
    public SSVEPBlinker blinkInstance;
    public Text countdownText;

    public float screenHz;
    public float timeToBlink;

    List<float> HzFactors = new List<float>();
    bool blockRunning;

    public Logger.Condition condition;
    SSVEPTrial currentTrial;
    SSVEPTrialBlock currentBlock;
    // Start is called before the first frame update
    void Start()
    {
        FactorizeHz();
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = (int)screenHz;
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (!firstTimeSetupRun)
        {
            SetupSquares();
            firstTimeSetupRun = true;
        }*/
        if (Input.GetKeyDown(KeyCode.Space) && !blockRunning)
        {
            /*foreach (GameObject g in SSVEPSquares)
            {
                StartBlinking(g);
            }*/
            BeginBlock();
        }
        else if (blockRunning && (!blinkInstance.enabled))
        {
            FinishTrial();
        }
    }

    void FactorizeHz()
    {
        int max = (int)Mathf.Sqrt(screenHz);
        for (int i = 0; i <= max; i++)
        {
            if (screenHz % i == 0) { HzFactors.Add(i); HzFactors.Add(screenHz / i); }
        }
    }

    void SetupSquares()
    {
        int i = 0;
        foreach (GameObject square in SSVEPSquares)
        {
            SSVEPBlinker blinkInstance = square.GetComponent<SSVEPBlinker>();
            blinkInstance.blinkDuration = timeToBlink;
            blinkInstance.hzToBlink = HzFactors[i];
            i++;
        }
    }

    void BeginBlock()
    {
        currentBlock = new SSVEPTrialBlock();
        blinkInstance.blinkDuration = timeToBlink;
        blockRunning = true;
        BeginTrial();
    }
    void BeginTrial()
    {
        blinkInstance.hzToBlink = HzFactors[currentBlock.GetTrialNum()];
        currentTrial = new SSVEPTrial(Time.time, blinkInstance.hzToBlink);
        StartBlinking(SSVEPSquare);
        Logger.SendMarker(blinkInstance.hzToBlink.ToString());
    }
    void FinishTrial()
    {
        currentTrial.endTime = Time.time;
        currentBlock.NextTrial();
        if (currentBlock.GetTrialNum() < HzFactors.Count) BeginTrial();
        else blockRunning = false;
    }
    void StartBlinking(GameObject square)
    {
        //SSVEPBlinker blinkInstance = square.GetComponent<SSVEPBlinker>();
        blinkInstance.startTime = Time.time;
        blinkInstance.enabled = true;
    }

    private IEnumerator countdown;
    private bool CountDown(int count)
    {
        if (count == 0) return true;
        else
        {
            countdownText.text = count.ToString();
            return false;
        }
    }
    #region Classes
    [System.Serializable]
    public class SSVEPTrial
    {
        public float startTime;
        public float endTime;
        public float Hz;

        public SSVEPTrial(float inStartTime, float inHz)
        {
            startTime = inStartTime;
            Hz = inHz;
        }
    }
    [System.Serializable]
    public class SSVEPTrialBlock
    {
        public List<SSVEPTrial> blockTrials;
        public List<float> HzFactors;
        int currentTrial = 0;
        public SSVEPTrialBlock()
        {
            blockTrials = new List<SSVEPTrial>();
        }
        public int GetTrialNum() { return currentTrial; }
        public void NextTrial() { currentTrial++; }
    }
    #endregion
}

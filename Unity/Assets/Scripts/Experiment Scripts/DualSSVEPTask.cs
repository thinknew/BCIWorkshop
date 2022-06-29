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

public class DualSSVEPTask : MonoBehaviour
{
    static public DualSSVEPTask instance;
    bool firstTimeSetupRun = false;

    public List<GameObject> SSVEPSquares;
    public GameObject SSVEPSquare;
    public SSVEPBlinker blinkInstance1;
    public SSVEPBlinker blinkInstance2;
    public Text countdownText;

    public float screenHz;
    public float timeToBlink;

    List<float> HzFactors = new List<float>();
    bool blockRunning;

    public Logger.Condition condition;

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
        if (Input.GetKeyDown(KeyCode.Space) && !blockRunning)
        {
            /*foreach (GameObject g in SSVEPSquares)
            {
                StartBlinking(g);
            }*/
            BeginBlock();
        }
        else if (blockRunning && (!blinkInstance1.enabled))
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
    void BeginBlock()
    {
        blinkInstance1.blinkDuration = timeToBlink;
        blinkInstance1.hzToBlink = HzFactors[5];

        blinkInstance2.blinkDuration = timeToBlink;
        blinkInstance2.hzToBlink = HzFactors[7];

        blockRunning = true;
        BeginTrial();
    }

    void BeginTrial()
    {
        foreach (GameObject g in SSVEPSquares)
       {
           StartBlinking(g);
       }
        Logger.SendMarker(blinkInstance1.hzToBlink.ToString()+"+"+ blinkInstance2.hzToBlink.ToString());
    }

    void FinishTrial()
    {
        blockRunning = false;
    }

    void StartBlinking(GameObject square)
    {
        SSVEPBlinker blinkInstance = square.GetComponent<SSVEPBlinker>();
        blinkInstance.startTime = Time.time;
        blinkInstance.enabled = true;
    }
}

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

public class P300Controller : MonoBehaviour
{
    #region Properties
    static public P300Controller instance;

    public Logger.Condition condition;
    P3Trial currentTrial;
    P3TrialBlock currentBlock;

    [Tooltip("Target Letter - Should be Capital Letter A-Z")]
    public char targetChar;

    [Tooltip("Time per trial in seconds (eg 0.2 = 200ms)")]
    public float timeToShowChar;

    [Tooltip("Number of letters per block (With only 1 target per block)")]
    public int charsPerBlock;
    char[] charArray;

    bool blockRunning = false;

    bool markLetters = false; //If false, sends 1 if target, 0 if not. If 1, sends the letter as a marker instead.

    public GameObject charText;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        GenerateCharArray(targetChar);
    }

    // Update is called once per frame
    void Update()
    {
        if (!blockRunning && Input.GetKeyDown(KeyCode.Space)) BeginBlock();
        if (blockRunning && Time.time >= currentTrial.startTime + timeToShowChar)
        {
            ProceedToNextTrial();
        }
    }
    
    char[] GenerateCharArray(char targetChar)
    {
        string nontargets = "abcdefghijklmnopqrstuvwxyz".ToUpper().Replace(targetChar.ToString(), string.Empty);
        char[] charArray = new char[charsPerBlock];
        for (int i = 0; i < charsPerBlock;  i++)
        {
            int roll = Random.Range(0, nontargets.Length);
            charArray[i] = nontargets[roll];
        }
        int targetRoll = Random.Range(0, charArray.Length);
        charArray[targetRoll] = targetChar;
        return charArray;
    }
    void BeginBlock()
    {
        currentBlock = new P3TrialBlock(GenerateCharArray(targetChar));
        SetupTrial();
        blockRunning = true;
    }
    void FinishBlock()
    {

    }

    void ProceedToNextTrial()
    {
        currentTrial.endTime = Time.time;
        currentBlock.blockTrials.Add(currentTrial);
        currentBlock.NextTrial();
        if (currentBlock.GetTrialNum() >= charsPerBlock)
        {
            blockRunning = false;
            charText.GetComponent<Text>().text = "End";
        }
        else SetupTrial();
    }
    void SetupTrial()
    {
        char inChar = currentBlock.GetCurrentLetter();
        currentTrial = new P3Trial(Time.time, inChar, (inChar == targetChar));      
        charText.GetComponent<Text>().text = inChar.ToString();
        if (markLetters) //Whether to send markers for letters, or for target/nontarget
        {
            Logger.SendMarker(inChar.ToString());
        }
        else
        {
            Logger.SendMarker(((inChar == targetChar) ? 1:0).ToString()); //Send 1 if inchar is target, else send 0
        }
    }

    #region Classes
    [System.Serializable]
    public class P3Trial
    {
        public float startTime;
        public float endTime;
        public char character;
        public bool isTarget;

        public P3Trial(float inStartTime, char inChar, bool inIsTarget)
        {
            startTime = inStartTime;
            character = inChar;
            isTarget = inIsTarget;
        }
    }
    [System.Serializable]
    public class P3TrialBlock
    {
        public List<P3Trial> blockTrials;
        public char[] charList;
        int currentTrial = 0;
       public P3TrialBlock(char[] inCharList)
        {
            blockTrials = new List<P3Trial>();
            charList = inCharList;
        }
        public char GetCurrentLetter() { return charList[currentTrial]; }
        public int GetTrialNum() { return currentTrial; }
        public void NextTrial() { currentTrial++; }
    }
    #endregion
}

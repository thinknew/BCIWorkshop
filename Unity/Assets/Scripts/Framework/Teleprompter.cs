using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Teleprompter : MonoBehaviour
{
    static public Teleprompter instance;
   public InputField messageField;
   public Text promptText;
    public float textDisappearTime;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (promptText == null) promptText = GameObject.FindGameObjectsWithTag("Prompt Text")[0].GetComponent<Text>();
    }

   public void DisplayCustomPrompt()
    {
        string inPrompt = messageField.text;
        promptText.text = inPrompt;
        promptText.enabled = true;
        Invoke("DisablePrompt", textDisappearTime);
        messageField.text = "";
    }
    public void DisplayPrompt (string inPrompt)
    {
        promptText.text = inPrompt;
        promptText.enabled = true;
        Invoke("DisablePrompt", textDisappearTime);
    }

    void DisablePrompt() { promptText.enabled = false; }
}

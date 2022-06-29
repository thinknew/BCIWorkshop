using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Valve.VR;
using UnityEngine.SceneManagement;

public class SceneMenu : MonoBehaviour
{
    //SteamVR_LoadLevel levelScript;
    // Start is called before the first frame update
    void Start()
    {
        //levelScript = transform.GetComponent<SteamVR_LoadLevel>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LoadLevel("Landing");
        }
    }

 public void LoadLevel(string inLevelName)
    {
        //levelScript.levelName = inLevelName;
        //levelScript.enabled = true;
        SceneManager.LoadScene(inLevelName);
    }
}

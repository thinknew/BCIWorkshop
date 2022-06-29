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

public class SSVEPBlinker : MonoBehaviour
{
    public float hzToBlink;
    public float startTime;
    public float blinkDuration;
    float lastStateChange;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnEnable()
    {
        if (hzToBlink == 0) this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > startTime + blinkDuration) EndBlinking();
        else if (Time.time > lastStateChange + 1 / hzToBlink) ChangeBlinkState();
    }
    void ChangeBlinkState()
    {
        Image image = this.GetComponent<Image>();
        Color col = image.color;
        col.a = 1 - col.a;
        image.color = col;
        lastStateChange = Time.time;
    }
    void EndBlinking()
    {
        Image image = this.GetComponent<Image>();
        Color col = image.color;
        col.a = 1;
        image.color = col;
        this.enabled = false;
    }
}

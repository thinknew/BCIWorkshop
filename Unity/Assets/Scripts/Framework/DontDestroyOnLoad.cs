using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    public string tag;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

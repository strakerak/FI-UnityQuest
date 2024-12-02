using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class testactivescript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject videogohesag;
    public GameObject videogohesag2;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void whenShitClicks()
    { 
        if (videogohesag.activeSelf)
            videogohesag.SetActive(false);
        else
            videogohesag.SetActive(true);

        if (videogohesag2.activeSelf)
            videogohesag2.SetActive(false);
        else
            videogohesag2.SetActive(true);

        Debug.Log(string.Format("{0} | Navigation | VideoHide Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        App.LogMessage(string.Format("{0} | Navigation | VideoHide Email Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));


    }
}

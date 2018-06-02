using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShowScreenshot : MonoBehaviour {

    Image image;

    public float changeCD = 10;
    public float nextChange = 0;

    private int index = 0;
    private void Awake()
    {
        image = GetComponent<Image>();
    }

    void Update () {

        if(ScreenShotKeeper.instance.screenShotList.Count>0)
        {
            if (Time.time > nextChange)
            {
                nextChange = Time.time + changeCD;
                image.sprite = ScreenShotKeeper.instance.screenShotList[index];

                index = (index + 1) % ScreenShotKeeper.instance.screenShotList.Count;
            }

        }

    }
}

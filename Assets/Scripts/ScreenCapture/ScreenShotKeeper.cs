using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShotKeeper : MonoBehaviour {


    public static ScreenShotKeeper instance = null;
    public List<Sprite> screenShotList;
    public int limit = 10;


    void Awake ()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
	
    public void addScreenShot(Sprite s)
    {
        if(screenShotList.Count>=limit)
        {
            int index = Random.Range(0, limit);
            screenShotList.RemoveAt(index);
        }

        screenShotList.Add(s);
    }
}

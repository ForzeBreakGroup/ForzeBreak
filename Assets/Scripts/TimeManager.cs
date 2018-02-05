using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Range(0.001f, 1.0f)]
    [SerializeField] private float slowdownFactor = 0.05f;
    [SerializeField] private int slowdownFrame = 5;
    private bool slowing = false;
    private int elapsedFrame = 0;
    private static TimeManager timeManager;
    public static TimeManager instance
    {
        get
        {
            if (!timeManager)
            {
                timeManager = FindObjectOfType<TimeManager>();
                if (!timeManager)
                {
                    Debug.LogError("TimeManager Script must be attached to a game object in scene");
                }
            }

            return timeManager;
        }
    }

    private void Update()
    {
        if (slowing)
        {
            ++elapsedFrame;
            if (elapsedFrame > slowdownFrame)
            {
                Time.timeScale = 1;
            }
        }
    }


    public void SlowMotion()
    {
        Time.timeScale = slowdownFactor;
        elapsedFrame = 0;
        slowing = true;
    }
}

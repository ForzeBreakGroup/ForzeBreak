using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private enum ESlowMotionEffect
    {
        FrameSlow,
        ShortPause
    };

    [Range(0.001f, 1.0f)]
    [SerializeField] private float slowdownFactor = 0.05f;
    [SerializeField] private int slowdownFrame = 5;
    [SerializeField] private bool enable = true;
    [SerializeField] private ESlowMotionEffect effectMode = ESlowMotionEffect.FrameSlow;
    
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

    public void SlowMotion()
    {
        if (enable)
        {
            switch (effectMode)
            {
                case ESlowMotionEffect.ShortPause:
                    StartCoroutine(ShortPauseEffect());
                    break;
                case ESlowMotionEffect.FrameSlow:
                default:
                    StartCoroutine(FrameSlowEffect());
                    break;
            }
        }
    }

    private IEnumerator FrameSlowEffect()
    {
        float waitTime = slowdownFrame * Time.fixedDeltaTime;
        Time.timeScale = slowdownFactor;

        yield return new WaitForSecondsRealtime(waitTime);

        Time.timeScale = 1;
    }

    private IEnumerator ShortPauseEffect()
    {
        float waitBeforePause = 10 * Time.fixedDeltaTime;
        float waitTime = slowdownFrame * Time.fixedDeltaTime;
        yield return new WaitForSecondsRealtime(waitBeforePause);

        Time.timeScale = slowdownFactor;

        yield return new WaitForSecondsRealtime(waitTime);

        Time.timeScale = 1;
    }
}

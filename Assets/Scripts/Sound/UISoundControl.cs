using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/*
 * Author: Robin Zhou
 * 
 * Description:
 * UI sound, Hover and Confirm Function are called by button on ui.
 */
public class UISoundControl : MonoBehaviour
{
    public FMOD.Studio.EventInstance BGM;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        BGM = FMODUnity.RuntimeManager.CreateInstance("event:/BGM/BGM");
        BGM.start();
    }

    public void onHover()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX_NonDiegetic/SFX_SwitchHover");
    }

    public void onConfirm()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX_NonDiegetic/SFX_SelectConfirm");

    }

    private void OnDestroy()
    {
        BGM.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        BGM.release();
    }
}
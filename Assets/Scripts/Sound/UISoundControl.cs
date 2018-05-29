﻿using UnityEngine;
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

    [FMODUnity.EventRef]
    [SerializeField]
    private string hoverSound;

    [FMODUnity.EventRef]
    [SerializeField]
    private string confirmSound;


    public void onHover()
    {
        FMODUnity.RuntimeManager.PlayOneShot(hoverSound);
    }

    public void onConfirm()
    {
        FMODUnity.RuntimeManager.PlayOneShot(confirmSound);
    }
}
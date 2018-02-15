using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Base class for all powerups, override OnPress, OnHold, OnRelease for handling the specific event you want
 * Awake must override from this class and calls base.Awake() for correctly getting the reticle system results
 */
public class PowerUpBase : Photon.MonoBehaviour
{
    protected ReticleSystem[] reticleTargets;

    protected virtual void Awake()
    {
        reticleTargets = GetComponentsInChildren<ReticleSystem>();
    }

    protected virtual void OnPress()
    {

    }

    protected virtual void OnHold()
    {

    }

    protected virtual void OnRelease()
    {

    }

    protected virtual void Update()
    {
        // Get input from player
        if (Input.GetButtonDown("Controller1_Button_Y"))
        {
            OnPress();
        }
        else if (Input.GetButtonUp("Controller1_Button_Y"))
        {
            OnRelease();
        }
        else if (Input.GetButton("Controller1_Button_Y"))
        {
            OnHold();
        }
    }
}

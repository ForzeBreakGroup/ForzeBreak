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
    private int playerNum;
    protected ReticleSystem[] reticleTargets;
    protected Dictionary<string, Vector3> weaponModelOffset;

    private void Awake()
    {
        playerNum = transform.root.gameObject.GetComponent<CarUserControl>().playerNum;
    }

    public virtual void AdjustModel()
    {
        reticleTargets = transform.root.gameObject.GetComponents<ReticleSystem>();
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
        if (Input.GetButtonDown("WeaponFire_Controller" + playerNum))
        {
            OnPress();
        }
        else if (Input.GetButtonUp("WeaponFire_Controller" + playerNum))
        {
            OnRelease();
        }
        else if (Input.GetButton("WeaponFire_Controller" + playerNum))
        {
            OnHold();
        }
    }
}

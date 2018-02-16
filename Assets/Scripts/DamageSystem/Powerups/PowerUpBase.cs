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
    public int playerNum = 0;
    protected ReticleSystem[] reticleTargets;

    public virtual void AdjustModel()
    {
        enabled = transform.root.gameObject.GetPhotonView().isMine;
        reticleTargets = transform.root.gameObject.GetComponentsInChildren<ReticleSystem>();
        playerNum = transform.root.gameObject.GetComponent<CarUserControl>().playerNum;
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
        if (Input.GetButtonDown("WeaponFire_Mouse") || Input.GetButtonDown("WeaponFire_Controller" + playerNum))
        {
            OnPress();
        }
        else if (Input.GetButtonUp("WeaponFire_Mouse") || Input.GetButtonUp("WeaponFire_Controller" + playerNum))
        {
            OnRelease();
        }
        else if (Input.GetButton("WeaponFire_Mouse") || Input.GetButton("WeaponFire_Controller" + playerNum))
        {
            OnHold();
        }
    }
}

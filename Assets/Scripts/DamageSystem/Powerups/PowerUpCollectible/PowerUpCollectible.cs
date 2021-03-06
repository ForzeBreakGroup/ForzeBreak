﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Powerup collectible behavior including the sound effect, visual effect and logic when trigger happened
 */
public class PowerUpCollectible : Photon.MonoBehaviour
{
    #region Members
    /// <summary>
    /// PowerUp Name assigned to this collectible, if random mode is on, it will generate a powerup name based on the tier level.
    /// The name must match a prefab name under Resources folder
    /// </summary>
    public string powerupName;

    /// <summary>
    /// Boolean indicator if power up has been collected
    /// </summary>
    public bool powerUpCollected = false;

    /// <summary>
    /// Boolean indicator if the collectible is in random mode
    /// </summary>
    [SerializeField]
    protected bool randomMode = true;

    /// <summary>
    /// Adjustable powerup tier level defined by PowreUpGrade class
    /// </summary>
    [SerializeField]
    protected PowerUpGrade.TierLevel powerUpTier = PowerUpGrade.TierLevel.COMMON;

    /// <summary>
    /// Instance of powerup grade for random power up generation
    /// </summary>
    protected PowerUpGrade powerUpGrade;

    /// <summary>
    /// FMOD sound effect instance
    /// </summary>
    private FMOD.Studio.EventInstance pickupSound;

    #endregion

    #region Methods
    protected void Awake()
    {
        powerUpGrade = new PowerUpGrade();
        pickupSound = FMODUnity.RuntimeManager.CreateInstance("event:/SFX_Diegetic/SFX_PowerupPickup");
    }

    /// <summary>
    /// TriggerEnter event that handles logic when a powerup has been picked up
    /// </summary>
    /// <param name="other"></param>
    protected void OnTriggerEnter(Collider other)
    {
        // Only player object can pick up powerup
        if (!powerUpCollected && other.transform.root.tag == "Player")
        {
            // Find the photonview ownership
            PhotonView view = other.transform.root.gameObject.GetPhotonView();
            if (view.isMine)
            {
                // Play sound effect
                FMODUnity.RuntimeManager.AttachInstanceToGameObject(pickupSound, transform, GetComponent<Rigidbody>());
                pickupSound.start();

                // Hide in remote client side to create illusion of powerup has been collected immediately, otherwise, it will have delay to wait for masterclient to destroy the object
                // Since the network delay is never perfectly zero, hiding the object will reduce the effect of delayed powerup collect
                foreach (Transform t in transform)
                {
                    Renderer rend = t.GetComponent<Renderer>();
                    if (rend)
                    {
                        rend.enabled = false;
                    }
                }
                transform.GetComponent<Renderer>().enabled = false;
                powerUpCollected = true;

                PowerUpCollected(view);
            }
        }
    }

    protected virtual void PowerUpCollected(PhotonView view)
    {

    }

    #endregion
}

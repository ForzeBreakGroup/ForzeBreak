﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileVersion1 : PowerUpBase
{
    [Range(0.1f, 1.0f)]
    [SerializeField] private float missileInterval = 0.5f;

    [Range(1, 5)]
    [SerializeField] private float missileDuration = 2f;

    private bool missileStartFiring = false;
    private float elapsedTime = 0.0f;

    private Dictionary<ReticleSystem, float> lockOnSystem;

    public override void AdjustModel()
    {
        base.AdjustModel();

        // Handling picking up same powerup
        MissileVersion1[] missileVersion1 = GetComponents<MissileVersion1>();
        if (missileVersion1.Length > 1)
        {
            DestroyImmediate(this);
        }

        lockOnSystem = new Dictionary<ReticleSystem, float>();
        foreach (ReticleSystem reticleSystem in reticleTargets)
        {
            lockOnSystem.Add(reticleSystem, 0);
        }

        // Move the weapon model to desired places
        transform.localPosition = new Vector3(2.4f, 0.7f, 0.25f);
        transform.localRotation = Quaternion.identity;
    }

    protected override void OnPress()
    {
        missileStartFiring = true;
    }

    private void FixedUpdate()
    {
        // If missle fire key has been pressed
        if (missileStartFiring)
        {
            List<ReticleSystem> keys = new List<ReticleSystem>(lockOnSystem.Keys);
            foreach(ReticleSystem key in keys)
            {
                float lockOnDuration;

                // If target is insight, then start the count down
                if (key.targetInSight)
                {
                    lockOnDuration = lockOnSystem[key];
                    lockOnDuration += Time.deltaTime;

                    // If countdown reached threshold, fire the missile
                    if (lockOnDuration > missileInterval)
                    {
                        // Fire a missile
                        FireMissileTowardsTarget(key.target);
                        lockOnDuration = 0;
                    }
                }
                // If target is not in sight, reset the countdown
                else
                {
                    lockOnDuration = 0;
                }

                lockOnSystem[key] = lockOnDuration;
            }

            elapsedTime += Time.deltaTime;
        }

        if (elapsedTime > missileDuration)
        {
            Destroy(this);
        }
    }

    private void FireMissileTowardsTarget(GameObject lockOnTarget)
    {
        GameObject missile = PhotonNetwork.Instantiate("Missile", transform.position, Quaternion.identity, 0);
        missile.GetComponent<MissileMovement>().target = lockOnTarget;
    }
}

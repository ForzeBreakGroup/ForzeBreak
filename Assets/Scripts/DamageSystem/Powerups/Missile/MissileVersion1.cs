using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileVersion1 : PowerUpComponent
{
    [Range(0.1f, 2.0f)]
    [SerializeField] private float missileInterval = 1.5f;

    [Range(1, 5)]
    [SerializeField] private float missileDuration = 2f;

    private bool missileStartFiring = false;
    private float elapsedTime = 0.0f;

    private Dictionary<ReticleSystem, float> lockOnSystem;

    private Transform launchLocation;

    protected override void Awake()
    {
        base.Awake();
        launchLocation = transform.Find("MissileLaunchPoint");
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
                    lockOnDuration += Time.fixedDeltaTime;

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

            elapsedTime += Time.fixedDeltaTime;
        }

        if (elapsedTime > missileDuration)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    private void FireMissileTowardsTarget(GameObject lockOnTarget)
    {
        GameObject missile = PhotonNetwork.Instantiate("Missile", launchLocation.position, Quaternion.identity, 0);
        missile.GetComponent<MissileMovement>().target = lockOnTarget;
        missile.GetComponent<MissileMovement>().Fire();
    }
}

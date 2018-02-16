using System.Collections;
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

        // Move the weapon model to desired places
        transform.localPosition = new Vector3(-0.03f, 0.35f, 0.25f);
        transform.localRotation = Quaternion.Euler(new Vector3(31, 0, 0));
    }

    protected override void OnPress()
    {
        missileStartFiring = true;
        PhotonNetwork.Instantiate("Missile", Vector3.zero, Quaternion.identity, 0);
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
                        Debug.Log(key.target);
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
        GameObject missile = PhotonNetwork.Instantiate("Missile", transform.position, Quaternion.identity, 1);
        missile.GetComponent<MissileMovement>().target = lockOnTarget;
    }
}

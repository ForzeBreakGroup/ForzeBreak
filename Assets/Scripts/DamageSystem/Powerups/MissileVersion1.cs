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

    private Dictionary<ReticleSystem, float> lockOnSystem;

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("Missile Equiped");
        Debug.Log(reticleTargets.Length);
        // Initialize the lock on system
        lockOnSystem = new Dictionary<ReticleSystem, float>();
        foreach (ReticleSystem rs in reticleTargets)
        {
            lockOnSystem.Add(rs, 0);
        }
    }

    protected override void OnPress()
    {
        missileStartFiring = true;
        Debug.Log("MissileStartFiring");
    }

    private void FixedUpdate()
    {
        // If missle fire key has been pressed
        if (missileStartFiring)
        {
            foreach (KeyValuePair<ReticleSystem, float> entry in lockOnSystem)
            {
                float lockOnDuration = entry.Value;
                lockOnDuration += Time.deltaTime;

                if (lockOnDuration > missileInterval)
                {
                    // Fire a missile
                    FireMissileTowardsTarget(entry.Key.target);
                    lockOnDuration = 0;
                }

                lockOnSystem[entry.Key] = lockOnDuration;
            }
        }
    }

    private void FireMissileTowardsTarget(GameObject lockOnTarget)
    {
        Debug.Log("Missile Fired");
        PhotonNetwork.Instantiate("Missile", Vector3.zero, Quaternion.identity, 1);
    }
}

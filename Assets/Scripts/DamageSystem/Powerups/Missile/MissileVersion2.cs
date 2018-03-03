using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissileVersion2 : PowerUpBase
{
    [Range(1, 10)]
    public int missileCapacity = 4;

    private List<ReticleSystem> lockOnSystem;

    private Transform launchLocation;

    private bool fired = false;

    private void Awake()
    {
        this.enabled = photonView.isMine;
        launchLocation = transform.Find("MissileLaunchPoint");
    }

    public override void AdjustModel()
    {
        base.AdjustModel();

        // Handling picking up same powerup
        MissileVersion1[] missileVersion1 = GetComponents<MissileVersion1>();
        if (missileVersion1.Length > 1)
        {
            DestroyImmediate(this);
        }

        lockOnSystem = new List<ReticleSystem>();
        foreach (ReticleSystem reticleSystem in reticleTargets)
        {
            lockOnSystem.Add(reticleSystem);
        }

        // Move the weapon model to desired places
        transform.localPosition = componentOffset;
        transform.localRotation = Quaternion.identity;
    }

    protected override void OnPress()
    {
        // Loop through each target
        foreach(ReticleSystem ret in lockOnSystem)
        {
            // If target is insight
            if (ret.targetInSight)
            {
                // Fire a missile
                ret.DisplayReticleUI(false);
                FireMissileTowardsTarget(ret.target);
                break;
            }
        }

        // If no targets were found, display missile no target animation
        if (!fired)
        {
            lockOnSystem[0].DisplayReticleUI(true);
        }

        // If all missiles fired, destroy the launcher
        --missileCapacity;
        if (missileCapacity <= 0)
        {
            UnloadPowerUp();
        }

        // Reset missile fire state
        fired = false;
    }

    private void FireMissileTowardsTarget(GameObject lockOnTarget)
    {
        GameObject missile = PhotonNetwork.Instantiate("Missile", launchLocation.position, Quaternion.identity, 0);
        missile.GetComponent<MissileMovement>().target = lockOnTarget;
        missile.GetComponent<MissileMovement>().Fire();
        fired = true;
    }
}

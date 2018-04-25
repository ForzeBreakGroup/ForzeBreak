using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballCollision : PowerUpCollision
{
    private float maxDamage;

    [SerializeField]
    [Range(0.1f, 1.0f)]
    private float damageRatioThreshold = 0.3f;

    private void Awake()
    {
        maxDamage = damage;
    }

    protected override void CollisionEnter(Collision collision)
    {
        base.CollisionEnter(collision);

        // Snowball does not apply damage to when the damage ratio threshold has not met
        if (((SnowballMovement)PowerUpMovement).CurrentScaleRatio() > damageRatioThreshold)
        {
            // The damage is potential maximum damage snowball can deal
            damage = CurrentDamage();

            ApplyDamage();
        }
    }

    private float CurrentDamage()
    {
        return maxDamage * ((SnowballMovement)PowerUpMovement).CurrentScaleRatio();
    }
}

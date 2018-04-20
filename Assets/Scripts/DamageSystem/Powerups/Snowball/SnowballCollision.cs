using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballCollision : PowerUpCollision
{
    private float maxDamage;

    private void Awake()
    {
        maxDamage = damage;
    }

    protected override void CollisionEnter(Collision collision)
    {
        base.CollisionEnter(collision);

        // The damage is potential maximum damage snowball can deal
        damage = CurrentDamage();

        ApplyDamage();
    }

    private float CurrentDamage()
    {
        return maxDamage * ((SnowballMovement)PowerUpMovement).CurrentScaleRatio();
    }
}

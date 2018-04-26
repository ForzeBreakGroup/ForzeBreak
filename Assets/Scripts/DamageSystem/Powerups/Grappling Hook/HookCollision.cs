using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookCollision : PowerUpCollision
{
    protected override void CollisionEnter(Collision collision)
    {
        base.CollisionEnter(collision);

        PowerUpMovement.DestroyPowerUpProjectile();
    }
}

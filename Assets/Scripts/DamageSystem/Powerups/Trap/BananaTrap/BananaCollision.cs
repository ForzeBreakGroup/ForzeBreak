using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaCollision : PowerUpCollision
{
    protected override void CollisionEnter(Collision collision)
    {
        base.CollisionEnter (collision);
        ApplyDamage("FakePowerUp");
        PowerUpMovement.DestroyPowerUpProjectile();
        PlayVFX();
    }
}

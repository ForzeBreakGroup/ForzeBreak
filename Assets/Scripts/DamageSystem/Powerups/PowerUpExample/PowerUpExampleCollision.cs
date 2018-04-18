using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpExampleCollision : PowerUpCollision
{
    protected override void CollisionEnter(Collision collision)
    {
        base.CollisionEnter(collision);

        DamageSystem dmgSystem = collision.transform.root.gameObject.GetComponent(typeof(DamageSystem)) as DamageSystem;
        if (dmgSystem)
        {
            dmgSystem.ApplyDamageForce(damage, this.transform.position + centerOffset, damageRadius);
        }
    }
}

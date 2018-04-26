using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionCollision : PowerUpCollision
{
    protected override void TriggerEnter(Collider other)
    {
        base.TriggerEnter(other);

        ApplyDamage();
    }
}

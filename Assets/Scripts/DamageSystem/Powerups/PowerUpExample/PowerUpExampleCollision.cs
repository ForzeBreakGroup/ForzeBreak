using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpExampleCollision : PowerUpCollision
{
    protected override void CollisionEnter(Collision collision)
    {
        ApplyDamage("Example");
    }

    protected override void TriggerEnter(Collider other)
    {
    }
}

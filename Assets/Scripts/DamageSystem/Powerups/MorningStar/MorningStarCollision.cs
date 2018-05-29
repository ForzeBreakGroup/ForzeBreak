using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorningStarCollision : PowerUpCollision
{
    protected override void CollisionEnter(Collision collision)
    {
        base.CollisionEnter(collision);
        GetComponent<PowerupSound>().PlaySound(1);
        ApplyDamage("MorningStar");
        transform.parent.GetComponent<MorningStarComponent>().DecreaseCapacity();
    }

    protected override void TriggerEnter(Collider other)
    {
    }
}

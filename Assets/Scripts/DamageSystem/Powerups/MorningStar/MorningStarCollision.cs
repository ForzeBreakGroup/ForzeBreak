using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorningStarCollision : PowerUpCollision
{
    protected override void CollisionEnter(Collision collision)
    {
        Debug.Log("Morning Star Collision");
        base.CollisionEnter(collision);
        ApplyDamage();
        transform.parent.GetComponent<MorningStarComponent>().DecreaseCapacity();
    }

    protected override void TriggerEnter(Collider other)
    {
    }
}

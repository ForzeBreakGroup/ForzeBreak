﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeRamCollision : PowerUpCollision
{
    protected override void CollisionEnter(Collision collision)
    {
        Debug.Log("Spike Ram Collision");
        base.CollisionEnter(collision);
        ApplyDamage();
        GetComponent<SpikeRamComponent>().DecreaseCapacity();
    }

    protected override void TriggerEnter(Collider other)
    {
    }
}

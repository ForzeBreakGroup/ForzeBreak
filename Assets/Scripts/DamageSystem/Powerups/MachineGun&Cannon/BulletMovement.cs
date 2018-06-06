using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Robin Zhou
 * 
 * Description:
 * Bullet moving class, bind to a single bullet.
 * 
 */
public class BulletMovement : PowerUpForceMove
{
    /// <summary>
    /// Bullet moving speed
    /// </summary>
    public float Velocity = 100f;

    protected override void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Velocity * transform.forward;
        spawnTime = Time.time;
    }
}

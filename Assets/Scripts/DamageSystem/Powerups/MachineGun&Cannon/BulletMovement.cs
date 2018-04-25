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
public class BulletMovement : PowerUpMovement
{
    /// <summary>
    /// Bullet moving speed
    /// </summary>
    public float Velocity = 100f;
    /// <summary>
    /// Bullet existing time duration
    /// </summary>
    public float ExistingTime = 3f;

    private float spawnTime = 0f;

    Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Velocity * transform.forward;
        spawnTime = Time.time;
    }

    private void Update()
    {
        if (Time.time > spawnTime + ExistingTime)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}

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
public class BulletMovement : NetworkPowerUpMovement
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
    protected override void Awake()
    {
        base.Awake();
        rb.velocity = Velocity * transform.forward;
        spawnTime = Time.time;
    }
    
    protected override void Move()
    {
        base.Move();

        if(Time.time>spawnTime+ExistingTime)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (photonView.isMine)
        {
            PhotonNetwork.Instantiate("Explosion1", transform.position, Quaternion.identity, 0);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}

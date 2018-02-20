using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : NetworkPowerUpMovement
{
    
    public float velocity = 100f;
    public float explosionRadius = 2.0f;
    public float explosionForce = 1000f;
    public float existingTime = 3f;

    private float spawnTime = 0f;
    protected override void Awake()
    {
        base.Awake();
        rb.velocity = velocity * transform.forward;
        spawnTime = Time.time;
    }
    
    protected override void Move()
    {
        base.Move();

        if(Time.time>spawnTime+existingTime)
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

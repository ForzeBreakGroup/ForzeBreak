using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GloveMovement : NetworkPowerUpMovement
{
    public float Velocity = 70f;
    public float ExistingTime = 3f;

    private float spawnTime = 0f;

    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();
        rb.velocity = Velocity * transform.forward;
        spawnTime = Time.time;
    }

    protected override void Move()
    {
        base.Move();

        if (Time.time > spawnTime + ExistingTime)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (photonView.isMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchingGloveMovement : PowerUpForceMove
{
    public float ExistingTime = 3f;

    private float spawnTime = 0f;

    // Use this for initialization
    protected override void Awake()
    {
		base.Awake ();
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
}

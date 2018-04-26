using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Carry
 * 
 * Description:
 * Move the Obj with power.
 */

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Transform))]
public class PowerUpForceMove : PowerUpMovement {

	protected Rigidbody rb;
	public float initVelocity = 0.0f;
	public float angle = 0.0f;
    public float ExistingTime = 3f;

    private float spawnTime = 0f;

    protected virtual void Awake()
	{
		rb = GetComponent<Rigidbody>();
		float corAngle = Mathf.PI * angle / 180;
		rb.velocity = initVelocity * transform.forward * Mathf.Cos (corAngle) + initVelocity * transform.up * Mathf.Sin (corAngle);
        spawnTime = Time.time;
    }

	protected virtual void FixedUpdate()
	{
		Move();

        if (Time.time > spawnTime + ExistingTime)
        {
            DestroyPowerUpProjectile();
        }
    }

	protected virtual void Move()
	{

	}
}

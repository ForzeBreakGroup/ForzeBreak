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

	protected virtual void Awake()
	{
		rb = GetComponent<Rigidbody>();
		float corAngle = Mathf.PI * angle / 180;
		rb.velocity = initVelocity * transform.forward * Mathf.Cos (corAngle) + initVelocity * transform.up * Mathf.Sin (corAngle);
	}

	protected virtual void FixedUpdate()
	{
		Move();
	}

	protected virtual void Move()
	{

	}
}

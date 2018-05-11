using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringCollision : PowerUpCollision
{
	protected override void CollisionEnter(Collision collision)
	{
		base.CollisionEnter (collision);
		float rangeFloatRight = Random.Range(-2.0f,2.0f);
		float rangeFloatForward = Random.Range (-2.0f,2.0f);
		this.transform.position = collision.transform.position + collision.transform.right * rangeFloatRight + collision.transform.forward*rangeFloatForward;
		ApplyDamage();
        PowerUpMovement.DestroyPowerUpProjectile();
	}
}

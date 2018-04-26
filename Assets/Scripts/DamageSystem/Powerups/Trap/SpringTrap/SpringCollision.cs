using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringCollision : TrapCollisionBase {
	protected override void CollisionEnter(Collision collision)
	{
		base.CollisionEnter (collision);

		this.transform.position = collision.transform.position;

		ApplyDamage();
		PhotonNetwork.Destroy(gameObject);
	}
}

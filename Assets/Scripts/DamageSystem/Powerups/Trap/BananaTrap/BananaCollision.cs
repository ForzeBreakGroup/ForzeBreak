using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaCollision : TrapCollisionBase {
	protected override void CollisionEnter(Collision collision)
	{
		base.CollisionEnter (collision);
		ApplyDamage();
		PhotonNetwork.Destroy(gameObject);
	}
}

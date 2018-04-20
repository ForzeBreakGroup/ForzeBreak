using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapCollisionBase : PowerUpCollision {
	protected override void CollisionEnter(Collision collision)
	{
		if (collision.transform.name == "New_Semi_Flat_arena") {
			return;
		}
	}
}

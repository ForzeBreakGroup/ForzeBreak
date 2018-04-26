using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchingGloveCollision : PowerUpCollision {
	protected override void CollisionEnter(Collision collision)
	{
		ApplyDamage();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchingGloveCollision : PowerUpCollision {
	protected override void CollisionEnter(Collision collision)
	{
		Debug.Log ("collison,,"+collision.transform.name);
		ApplyDamage();
		PhotonNetwork.Destroy(gameObject);
	}
}

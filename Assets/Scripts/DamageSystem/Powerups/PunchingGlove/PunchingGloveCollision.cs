using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PunchingGloveCollision : PowerUpCollision {

	protected override void CollisionEnter(Collision collision)
	{
		InGameHUDManager.instance.ShowKOUI ();
		Debug.Log ("collison,,"+collision.transform.name);
		ApplyDamage();
	}
}

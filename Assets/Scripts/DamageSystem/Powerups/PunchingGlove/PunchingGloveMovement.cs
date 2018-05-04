using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchingGloveMovement : PowerUpForceMove
{
	public float finalScale = 10.0f;
	protected override void Move()
	{
		float curScale = 1.0f + (Time.time - spawnTime) * (finalScale - 1.0f);
		transform.localScale = new Vector3 (curScale,curScale,curScale);
	}
}

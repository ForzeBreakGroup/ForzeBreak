using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour {

    public float duration = 0.5f;
    private float t = 0;
	void Update () {
		if(t>duration)
        {
            Destroy(gameObject);
        }else
        {
            t += Time.deltaTime;
        }
	}
}

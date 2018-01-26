using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiminishArena : MonoBehaviour {
    float speed = 0.01f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(transform.localScale.magnitude>0.01)
             transform.localScale -= new Vector3(speed * Time.deltaTime, speed * Time.deltaTime, speed * Time.deltaTime);
	}
}

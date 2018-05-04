using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIKOControl : MonoBehaviour {

	public float finalScale = 3.0f;
	private Text KOText;
	private float scaleTime = 0.2f;
	private float lastTime = 1.0f;
	private float activeTime;

	private void Awake()
	{
		KOText = GetComponent<Text>();
		KOText.enabled = false;
	}

	public void setUIActive()
	{
		transform.localScale = new Vector3 (0.0f,0.0f,0.0f);
		transform.localPosition = new Vector3 (0.0f,0.0f,0.0f);
		activeTime = Time.time;
		KOText.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (KOText.enabled) {
			if (Time.time <= activeTime + scaleTime) {
				float curScale = finalScale * (Time.time - activeTime)/scaleTime;
				transform.localScale = new Vector3 (curScale, curScale, curScale);
			}

			if (Time.time > activeTime + scaleTime && Time.time <= activeTime + lastTime) {
				transform.localPosition = 30 * Random.insideUnitCircle * (activeTime + lastTime - Time.time) / (lastTime - scaleTime);
			}

			if (Time.time > activeTime + lastTime) {
				KOText.enabled = false;
			}

		}
	}
}

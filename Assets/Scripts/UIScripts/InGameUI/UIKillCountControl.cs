using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIKillCountControl : MonoBehaviour
{
    private Text killCount;
	private float scale_1_time = 1.0f;
	private float scale_2_time = 2.0f;
	private Vector3 oriPos;

	private float runBeginTime;

	private float finalScale = 3.0f;

	private bool needUpdate = false;

	private static UIKillCountControl uiKillController;
	public static UIKillCountControl instance
	{
		get
		{
			if (!uiKillController)
			{
				uiKillController = FindObjectOfType(typeof(UIKillCountControl)) as UIKillCountControl;
				if (!uiKillController)
				{
					Debug.LogError("UIKillCountControl script must be attached to an active GameObject in scene");
				}
				else
				{
					uiKillController.Init();
				}
			}


			return uiKillController;
		}
	}

	private void Init()
    {
        killCount = GetComponent<Text>();
		oriPos = transform.position;
    }

	private void Update()
	{
		if (needUpdate) {
			if (Time.time <= runBeginTime + scale_1_time) {
				float curScale = finalScale * (Time.time - runBeginTime) / scale_1_time;
				transform.localScale = new Vector3 (curScale, curScale, curScale);
			} else if (Time.time <= runBeginTime + scale_1_time + scale_2_time) {
				float curScale = finalScale - (finalScale - 1.0f) * (Time.time - runBeginTime - scale_1_time) / scale_2_time;
				transform.localScale = new Vector3 (curScale, curScale, curScale);

				transform.position = Vector3.Lerp (transform.position, oriPos, 0.05f);
			} else {
				needUpdate = false;
			}
		}
	}

	public void UpdateCount()
    {
		killCount.text = ((int)PhotonNetwork.player.CustomProperties ["KillCount"]).ToString ();
		transform.localScale = new Vector3 (0.0f,0.0f,0.0f);
		transform.position = new Vector3 (Screen.width*0.5f,Screen.height*0.5f,transform.position.z);
		runBeginTime = Time.time;
		needUpdate = true;
    }
}

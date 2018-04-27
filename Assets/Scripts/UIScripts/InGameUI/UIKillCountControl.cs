using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIKillCountControl : MonoBehaviour
{
    private Text killCount;

    private void Awake()
    {
        killCount = GetComponent<Text>();
    }

    private void Update()
    {
        killCount.text = ((int)PhotonNetwork.player.CustomProperties["KillCount"]).ToString();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerScoreDisplay : MonoBehaviour
{
    [SerializeField]
    private int playerScoreIndex = 0;
    private Text nameText;

    private void Awake()
    {
        nameText = GetComponent<Text>();
    }

    private void Update()
    {
        PhotonPlayer p = UIPlayerScoreTracker.instance.GetPlayerAtPlace(playerScoreIndex);
        if (p != null)
        {
            nameText.text = p.NickName + ": " + ((int)p.CustomProperties["KillCount"]).ToString();

            float[] serializeColor = p.CustomProperties["Color"] as float[];
            nameText.color = new Color(serializeColor[0], serializeColor[1], serializeColor[2], serializeColor[3]);
        }
    }
}

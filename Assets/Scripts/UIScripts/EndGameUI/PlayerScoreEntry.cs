using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreEntry : MonoBehaviour
{
    private Text playerName;
    private Text killScore;

    private void Awake()
    {
        playerName = transform.Find("PlayerName").GetComponent<Text>();
        killScore = transform.Find("KillScore").GetComponent<Text>();
    }

    public void UpdateText(string playerName, int killCount)
    {
        this.playerName.text = playerName;
        killScore.text = killCount.ToString();
    }
}

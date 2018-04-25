using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField]
    private GameObject scoreEntry;

    private void Awake()
    {
        foreach (PhotonPlayer p in PhotonNetwork.playerList)
        {
            GameObject entry = Instantiate(scoreEntry, this.transform);
            PlayerScoreEntry playerScoreEntry = entry.GetComponent<PlayerScoreEntry>();
            playerScoreEntry.UpdateText(p.NickName, (int)p.CustomProperties["KillCount"]);
        }
    }
}

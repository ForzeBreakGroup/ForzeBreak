using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField]
    private GetWinnerName winnerName;

    private PlayerScoreCard[] scoreCards;

    [HideInInspector]
    public List<PhotonPlayer> sortedPlayerScore;

    private void Awake()
    {
        scoreCards = GetComponentsInChildren<PlayerScoreCard>();

        // Sort player by score, highest at index 0
        sortedPlayerScore = new List<PhotonPlayer>();
        SortPlayerByScore();

        // Loop to set individual card with information
        for(int i = 0; i < sortedPlayerScore.Count; ++i)
        {
            PlayerScoreCard psc = scoreCards[i];
            psc.SetPlayerInfo(sortedPlayerScore[i].NickName, (int)sortedPlayerScore[i].CustomProperties["KillCount"]);
        }
    }

    private void SortPlayerByScore()
    {
        // Iterate through all players in game session and sort it accordingly
        foreach (PhotonPlayer p in PhotonNetwork.playerList)
        {
            int score = (int)p.CustomProperties["KillCount"];
            int index = 0;

            // Insertion sort
            for (index = 0; index < sortedPlayerScore.Count; ++index)
            {
                if ((int)sortedPlayerScore[index].CustomProperties["KillCount"] < score)
                {
                    break;
                }
            }

            sortedPlayerScore.Insert(index, p);
        }

        // Debug output
        foreach(PhotonPlayer p in sortedPlayerScore)
        {
            Debug.Log(p.NickName);
            Debug.Log(p.CustomProperties["KillCount"]);
        }
    }

    public void OnScoreBoardIntroComplete()
    {
        for (int i = 0; i < sortedPlayerScore.Count; ++i)
        {
            scoreCards[i].Flip();
        }
        winnerName.OnDisplayWinnerName(sortedPlayerScore[0].NickName);
    }
}

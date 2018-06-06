using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerScoreTracker : MonoBehaviour
{
    private List<PhotonPlayer> playerScoreList;
    private static UIPlayerScoreTracker playerScoreTracker;
    public static UIPlayerScoreTracker instance
    {
        get
        {
            if (!playerScoreTracker)
            {
                playerScoreTracker = FindObjectOfType<UIPlayerScoreTracker>();
                if (!playerScoreTracker)
                {
                    Debug.LogError("UIPlayerScoreTracker must be attached to an active gameobject in scene.");
                }
            }

            return playerScoreTracker;
        }
    }

    private void Awake()
    {
        playerScoreList = new List<PhotonPlayer>();

        foreach (PhotonPlayer p in PhotonNetwork.playerList)
        {
            playerScoreList.Add(p);
        }
    }

    private void Update()
    {
        int i = 0;
        int j = 0;
        for (i = 0; i < playerScoreList.Count - 1; i++)
        {
            int iKillCount = (int)playerScoreList[i].CustomProperties["KillCount"];
            int max = i;

            for (j = i + 1; j < playerScoreList.Count; j++)
            {
                int jKillCount = (int)playerScoreList[j].CustomProperties["KillCount"];
                if (jKillCount > iKillCount)
                {
                    max = j;
                }
            }

            if (max != i)
            {
                PhotonPlayer p = playerScoreList[i];
                playerScoreList[i] = playerScoreList[max];
                playerScoreList[max] = p;
            }
        }
    }

    public PhotonPlayer GetPlayerAtPlace(int i)
    {
        if (i >= playerScoreList.Count)
        {
            return null;
        }
        return playerScoreList[i];
    }
}

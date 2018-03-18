using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;

public class LobbyUI : Photon.MonoBehaviour
{
    private bool isReady;
    private int numberOfPlayers = 1;

    Text readyText;
    Text notReadyText;
    Text playerConn;
    Text winnerText;

    private void Awake()
    {
        readyText = transform.Find("ReadyText").GetComponent<Text>();
        notReadyText = transform.Find("NotReadyText").GetComponent<Text>();
        playerConn = transform.Find("PlayerConn").GetComponent<Text>();
        winnerText = transform.Find("WinnerText").GetComponent<Text>();

        readyText.enabled = isReady;
        notReadyText.enabled = !isReady;

        if(NetworkManager.offlineMode)
        {
            gameObject.SetActive(false);
        }
        else
        {
            NetworkDisplay();
        }
    }

    private void LocalDisplay()
    {
        playerConn.text = "";
    }

    public void NetworkDisplay()
    {
        playerConn.text = PhotonNetwork.playerList.Length + " / 4";
        if (PhotonNetwork.playerList.Length > 1)
        {
            playerConn.color = Color.green;
        }
        else
        {
            playerConn.color = Color.red;
        }
    }

    private void OnEnable()
    {
        isReady = false;
        readyText.enabled = isReady;
        notReadyText.enabled = !isReady;
        playerConn.text = PhotonNetwork.playerList.Length + " / 4";
    }

    public void DisplayWinner(int winnerID)
    {
        if (winnerID == -2)
        {
            winnerText.text = "Get READY for ForzeBreak!";
        }

        else if (winnerID == -1)
        {
            winnerText.text = "Close Game! It was a tie";
        }

        else
        {
            if ((winnerID - 1) == (int)PhotonNetwork.player.CustomProperties["PlayerNumber"])
            {
                winnerText.text = "Victory!";
            }
            else
            {
                winnerText.text = string.Format("Winner is Player {0}", winnerID);
            }
        }
    }

    public void ReadyState()
    {
        isReady = !isReady;
        readyText.enabled = isReady;
        notReadyText.enabled = !isReady;

        MatchManager.instance.photonView.RPC("RpcPlayerReadyHandler", PhotonTargets.All, NetworkManager.instance.GetLocalPlayer().GetPhotonView());
    }
}

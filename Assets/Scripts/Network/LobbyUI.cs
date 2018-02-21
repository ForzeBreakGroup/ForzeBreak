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

    private void Awake()
    {
        readyText = transform.Find("ReadyText").GetComponent<Text>();
        notReadyText = transform.Find("NotReadyText").GetComponent<Text>();
        playerConn = transform.Find("PlayerConn").GetComponent<Text>();

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
        PhotonNetwork.OnEventCall += EvtAddPlayerToMatchHandler;
        PhotonNetwork.OnEventCall += EvtRemovePlayerFromMatchHandler;

        isReady = false;
        readyText.enabled = isReady;
        notReadyText.enabled = !isReady;
        playerConn.text = PhotonNetwork.playerList.Length + " / 4";
    }

    private void OnDisable()
    {
        PhotonNetwork.OnEventCall -= EvtAddPlayerToMatchHandler;
        PhotonNetwork.OnEventCall -= EvtRemovePlayerFromMatchHandler;
    }

    private void EvtAddPlayerToMatchHandler(byte evtCode, object content, int senderid)
    {
        if (evtCode == (byte)ENetworkEventCode.OnAddPlayerToMatch)
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
    }

    private void EvtRemovePlayerFromMatchHandler(byte evtCode, object content, int senderid)
    {
        if (evtCode == (byte)ENetworkEventCode.OnRemovePlayerFromMatch)
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
    }

    public void ReadyState()
    {
        isReady = !isReady;
        readyText.enabled = isReady;
        notReadyText.enabled = !isReady;

        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.MasterClient;
        PhotonNetwork.RaiseEvent((byte)ENetworkEventCode.OnPlayerReady, isReady, true, options);
    }
}

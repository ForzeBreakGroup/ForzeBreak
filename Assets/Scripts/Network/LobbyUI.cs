using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;

public class LobbyUI : Photon.MonoBehaviour
{
    private bool isReady = false;
    private int numberOfPlayers = 1;

    Text readyText;
    Text notReadyText;
    Text playerConn;

    private void Awake()
    {
        readyText = transform.Find("ReadyText").GetComponent<Text>();
        notReadyText = transform.Find("NotReadyText").GetComponent<Text>();
        playerConn = transform.Find("PlayerConn").GetComponent<Text>();

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

    private void NetworkDisplay()
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
        PhotonNetwork.OnEventCall += EvtAddPlayerToMatchHandler;
        PhotonNetwork.OnEventCall += EvtRemovePlayerFromMatchHandler;
        PhotonNetwork.OnEventCall += EvtPlayersSpawningHandler;
    }

    private void OnDisable()
    {
        PhotonNetwork.OnEventCall -= EvtAddPlayerToMatchHandler;
        PhotonNetwork.OnEventCall -= EvtRemovePlayerFromMatchHandler;
        PhotonNetwork.OnEventCall -= EvtPlayersSpawningHandler;
    }

    private void EvtAddPlayerToMatchHandler(byte evtCode, object content, int senderid)
    {
        if (evtCode == (byte)ENetworkEventCode.OnAddPlayerToMatch)
        {
            NetworkDisplay();
        }
    }

    private void EvtRemovePlayerFromMatchHandler(byte evtCode, object content, int senderid)
    {
        if (evtCode == (byte)ENetworkEventCode.OnRemovePlayerFromMatch)
        {
            NetworkDisplay();
        }
    }

    private void EvtPlayersSpawningHandler(byte evtCode, object content, int senderid)
    {
        if (evtCode == (byte)ENetworkEventCode.OnPlayerSpawning)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void ReadyState()
    {
        isReady = !isReady;
        readyText.enabled = isReady;
        notReadyText.enabled = !isReady;

        if (NetworkManager.offlineMode)
        {
            EventManager.TriggerEvent("OnPlayerReady");
        }
        else
        {
            RaiseEventOptions options = new RaiseEventOptions();
            options.Receivers = ReceiverGroup.MasterClient;
            PhotonNetwork.RaiseEvent((byte)ENetworkEventCode.OnPlayerReady, isReady, true, options);
        }
    }
}

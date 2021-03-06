﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : Photon.MonoBehaviour
{
    public delegate void LobbyManagerPlayerListUpdateNotify();
    public static LobbyManagerPlayerListUpdateNotify playerListUpdateCallbackFunc;

    [SerializeField]
    private int countdownSec = 3;

    public Dictionary<PhotonPlayer, bool> playerReadyStatus { get; private set; }

    private static LobbyManager lobbyManager;
    public static LobbyManager instance
    {
        get
        {
            if (!lobbyManager)
            {
                lobbyManager = FindObjectOfType<LobbyManager>();
                if (!lobbyManager)
                {
                    Debug.LogError("LobbyManager script must be attached to an active gameobject in scene");
                }
                else
                {
                    lobbyManager.Init();
                }
            }

            return lobbyManager;
        }
    }

    private void OnEnable()
    {
        EventManager.StartListening("EvtOnPlayerConnected", EvtOnPlayerConnectedHandler);
        EventManager.StartListening("EvtOnPLayerDisconnected", EvtOnPlayerDisconnectedHandler);
    }

    private void OnDisable()
    {
        EventManager.StopListening("EvtOnPlayerConnected", EvtOnPlayerConnectedHandler);
        EventManager.StopListening("EvtOnPLayerDisconnected", EvtOnPlayerDisconnectedHandler);
    }

    private void Awake()
    {
        foreach (PhotonPlayer p in  PhotonNetwork.playerList)
        {
            ExitGames.Client.Photon.Hashtable killCount = new ExitGames.Client.Photon.Hashtable() { { "KillCount", 0 } };
            p.SetCustomProperties(killCount);
        }
    }

    private void Init()
    {
        playerReadyStatus = new Dictionary<PhotonPlayer, bool>();

        foreach (PhotonPlayer p in PhotonNetwork.playerList)
        {
            playerReadyStatus.Add(p, false);
        }
    }

    private void EvtOnPlayerConnectedHandler()
    {
        foreach(PhotonPlayer p in PhotonNetwork.playerList)
        {
            if (!playerReadyStatus.ContainsKey(p))
            {
                playerReadyStatus.Add(p, false);
            }
        }

        playerListUpdateCallbackFunc();
    }

    private void EvtOnPlayerDisconnectedHandler()
    {
        Dictionary<PhotonPlayer, bool> newPlayerReadyStatus = new Dictionary<PhotonPlayer, bool>();

        foreach(PhotonPlayer p in PhotonNetwork.playerList)
        {
            newPlayerReadyStatus.Add(p, playerReadyStatus[p]);
        }

        playerReadyStatus = newPlayerReadyStatus;

        playerListUpdateCallbackFunc();
    }

    #region UI OnClick Events
    public void OnPlayerClickReady()
    {
        photonView.RPC("RpcTogglePlayerReady", PhotonTargets.AllBuffered, PhotonNetwork.player);
    }
    #endregion

    #region Photon RPC Calls
    [PunRPC]
    public void RpcTogglePlayerReady(PhotonPlayer player)
    {
        playerReadyStatus[player] = !playerReadyStatus[player];

        playerListUpdateCallbackFunc();

        // Master client will handle the scene transition when all players are ready
        if (PhotonNetwork.isMasterClient)
        {
            // Check every player, if anyone is not ready, exit execution
            foreach(KeyValuePair<PhotonPlayer, bool> entry in playerReadyStatus)
            {
                if (!entry.Value)
                {
                    return;
                }
            }

            // Close the room to prevent new players to join
            NetworkManager.instance.EnableTheRoom(false);

            // Start countdown UI

            // Start coroutine for load scene
            StartCoroutine(LoadSceneAfterDelay());
        }
    }

    IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        PhotonNetwork.LoadLevel("Online");
    }
    #endregion
}

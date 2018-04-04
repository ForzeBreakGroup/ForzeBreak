using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : Photon.MonoBehaviour
{
    public Dictionary<string, bool> playerReadyStatus { get; private set; }

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
        EventManager.StartListening("OnPhotonPlayerConnected", OnPhotonPlayerConnectedHandler);
        EventManager.StartListening("OnPhotonPlayerDisconnected", OnPhotonPlayerDisconnectedHandler);
    }

    private void OnDisable()
    {
        EventManager.StopListening("OnPhotonPlayerConnected", OnPhotonPlayerConnectedHandler);
        EventManager.StopListening("OnPhotonPlayerDisconnected", OnPhotonPlayerDisconnectedHandler);
    }

    private void Awake()
    {
        playerReadyStatus = new Dictionary<string, bool>();
        foreach (PhotonPlayer p in PhotonNetwork.playerList)
        {
            Debug.Log(p.NickName);
            playerReadyStatus.Add(p.NickName, false);
        }
        Debug.Log(PhotonNetwork.player.NickName);
    }

    private void OnPhotonPlayerConnectedHandler()
    {
        foreach(PhotonPlayer p in PhotonNetwork.playerList)
        {
            if (!playerReadyStatus.ContainsKey(p.NickName))
            {
                playerReadyStatus.Add(p.NickName, false);
            }
        }
    }

    private void OnPhotonPlayerDisconnectedHandler()
    {
        Dictionary<string, bool> newPlayerStatus = new Dictionary<string, bool>();

        foreach(PhotonPlayer p in PhotonNetwork.playerList)
        {
            newPlayerStatus.Add(p.NickName, false);
        }
        playerReadyStatus = newPlayerStatus;
    }

    private void Init()
    {
    }

    public void OnClickReady()
    {
        photonView.RPC("RpcTogglePlayerReady", PhotonTargets.All, PhotonNetwork.player.NickName);
    }

    #region Photon RPC Calls
    [PunRPC]
    public void RpcTogglePlayerReady(string nickname)
    {
        playerReadyStatus[nickname] = !playerReadyStatus[nickname];

        // Master client will handle the scene transition when all players are ready
        if (PhotonNetwork.isMasterClient)
        {
            // Loop through dictionary to verify all players are ready
            foreach(KeyValuePair<string, bool> entry in playerReadyStatus)
            {
                // Early return if any value is false
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

    [PunRPC]
    public void RpcStartCountdown()
    {

    }

    IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        PhotonNetwork.LoadLevel("Online");
    }
    #endregion
}

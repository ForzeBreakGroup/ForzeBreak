using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : Photon.MonoBehaviour
{
    [SerializeField]
    private int countdownSec = 3;

    public Dictionary<PhotonPlayer, bool> playerReadyStatus { get; private set; }

    public delegate void LobbyManagerPlayerJoinLobby(PhotonPlayer player);
    public static LobbyManagerPlayerJoinLobby LobbyManagerPlayerJoinLobbyCallbackFunc;

    public delegate void LobbyManagerPlayerLeftLobby(PhotonPlayer player);
    public static LobbyManagerPlayerLeftLobby LobbyManagerPlayerLeftLobbyCallbackFunc;

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
        NetworkManager.NetworkManagerPlayerConnectGameCallbackFunc = EvtOnPlayerConnectedToGame;
        NetworkManager.NetworkManagerPlayerDisconnectGameCallbackFunc = EvtOnPlayerDisconnectFromGame;
    }

    private void Awake()
    {
        foreach (PhotonPlayer p in  PhotonNetwork.playerList)
        {
            ExitGames.Client.Photon.Hashtable killCount = new ExitGames.Client.Photon.Hashtable() { { "KillCount", 0 } };
            p.SetCustomProperties(killCount);
        }

        playerReadyStatus = new Dictionary<PhotonPlayer, bool>();

        foreach (PhotonPlayer p in PhotonNetwork.playerList)
        {
            EvtOnPlayerConnectedToGame(p);
        }
    }


    private void Init()
    {
    }

    private void EvtOnPlayerConnectedToGame(PhotonPlayer p)
    {
        if (!playerReadyStatus.ContainsKey(p))
        {
            playerReadyStatus.Add(p, false);
            LobbyManagerPlayerJoinLobbyCallbackFunc(p);
        }
    }

    private void EvtOnPlayerDisconnectFromGame(PhotonPlayer p)
    {
        if (playerReadyStatus.ContainsKey(p))
        {
            playerReadyStatus.Remove(p);
            LobbyManagerPlayerLeftLobbyCallbackFunc(p);
        }
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

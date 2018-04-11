using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameLobbyManager : Photon.MonoBehaviour
{
    private int numberOfPlayerReady = 0;
    private Dictionary<PhotonPlayer, bool> rematchPlayers;

    private static EndGameLobbyManager endGameLobbyManager;
    public static EndGameLobbyManager instance
    {
        get
        {
            if (!endGameLobbyManager)
            {
                endGameLobbyManager = FindObjectOfType<EndGameLobbyManager>();
                if (!endGameLobbyManager)
                {
                    Debug.LogError("EndGameLobbyManager must be attached to an active gameobject in scene");
                }
                else
                {
                    endGameLobbyManager.Init();
                }
            }

            return endGameLobbyManager;
        }
    }

    private void Awake()
    {
        rematchPlayers = new Dictionary<PhotonPlayer, bool>();
        foreach (PhotonPlayer p in PhotonNetwork.playerList)
        {
            rematchPlayers.Add(p, false);
        }
    }

    private void Init()
    {
    }

    public void RegisterForRematch(bool isRematching)
    {
        photonView.RPC("PlayerReadyForRematch", PhotonTargets.AllBuffered, isRematching, PhotonNetwork.player);
    }

    [PunRPC]
    public void PlayerReadyForRematch(bool isRematching, PhotonPlayer sender)
    {
        // Register player for rematch
        rematchPlayers[sender] = isRematching;

        // Following code will be executed by master client
        if (PhotonNetwork.isMasterClient)
        {
            // If any player exit the match, close the room
            if (!isRematching)
            {
                photonView.RPC("LeaveRoom", PhotonTargets.AllBuffered);
            }

            foreach(KeyValuePair<PhotonPlayer, bool> entry in rematchPlayers)
            {
                if (!entry.Value)
                {
                    return;
                }

                PhotonNetwork.LoadLevel("MatchLobby");
            }
        }
    }

    [PunRPC]
    public void LeaveRoom()
    {
        // Exit the room
        PhotonNetwork.LeaveRoom();
    }
}

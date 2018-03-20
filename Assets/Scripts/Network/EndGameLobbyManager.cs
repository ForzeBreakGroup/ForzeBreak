using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameLobbyManager : Photon.MonoBehaviour
{
    private int numberOfPlayerReady = 0;

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


    private void Init()
    {

    }

    public void RegisterForRematch(bool isRematching)
    {
        photonView.RPC("PlayerReadyForRematch", PhotonTargets.AllBuffered, isRematching);
    }

    [PunRPC]
    public void PlayerReadyForRematch(bool isRematching)
    {
        // Register the number of player ready for rematch
        if (isRematching)
        {
            ++numberOfPlayerReady;
        }

        // Following code will be executed by master client
        if (PhotonNetwork.isMasterClient)
        {
            // If any player exit the match, close the room
            if (!isRematching)
            {
                photonView.RPC("LeaveRoom", PhotonTargets.AllBuffered);
            }

            // If all players wants to rematch, return to match lobby
            if (numberOfPlayerReady == PhotonNetwork.playerList.Length)
            {
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : Photon.MonoBehaviour
{
    [SerializeField]
    private int countdownSec = 3;

    private int numOfReady = 0;

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

    private void Awake()
    {
        // Asyn load match scene for smooth transition
    }

    private void Init()
    {

    }

    #region UI OnClick Events
    public void OnClickReady()
    {
        photonView.RPC("RpcPlayerReady", PhotonTargets.All);

        // UI change to indicate the selection is locked

        // Disable selection
    }
    #endregion

    #region Photon RPC Calls
    [PunRPC]
    public void RpcPlayerReady()
    {
        // All clients will keep an update of current number of ready
        ++numOfReady;

        // Master client will handle the scene transition when all players are ready
        if (PhotonNetwork.isMasterClient && numOfReady == PhotonNetwork.playerList.Length)
        {
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

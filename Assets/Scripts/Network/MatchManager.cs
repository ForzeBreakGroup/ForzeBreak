using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchManager : Photon.MonoBehaviour
{
    public GameObject cam;
    private NetworkSpawnPoint[] spawnPoints;
    private Dictionary<int, bool> playersStillAlive;
    private Dictionary<int, bool> playersInScene;
    private GameObject lobbyUI;
    private string playerPrefabName = "War_Buggy";
    private int numOfLocalPlayers = 0;

    private static MatchManager matchManager;
    public static MatchManager instance
    {
        get
        {
            if (!matchManager)
            {
                matchManager = FindObjectOfType(typeof(MatchManager)) as MatchManager;
                if (!matchManager)
                {
                    Debug.LogError("MatchManager Script must be attached to an active GameObject in Scene");
                }
                else
                {
                    matchManager.Init();
                }
            }

            return matchManager;
        }
    }

    private void Init()
    {
        lobbyUI = FindObjectOfType<Canvas>().transform.Find("Lobby").gameObject;
    }

    private void Awake()
    {
        // Initialize dictioanry for tracking players still alive in match
        playersStillAlive = new Dictionary<int, bool>();
        playersInScene = new Dictionary<int, bool>();

        foreach(PhotonPlayer player in PhotonNetwork.playerList)
        {
            playersStillAlive.Add(player.ID, true);
            playersInScene.Add(player.ID, false);
        }

        // Find the spawn points
        spawnPoints = FindObjectsOfType<NetworkSpawnPoint>();

        // Validate camera object is attached to script
        if (!cam)
        {
            Debug.LogError("Camera Not Attached");
        }

        // Photon RPC call to let everyone know this client has joined the game
        photonView.RPC("RpcPlayerJoinedSceneHandler", PhotonTargets.AllBufferedViaServer, PhotonNetwork.player.ID);
    }

    private void SpawnPlayer()
    {
        // Obtain the vehicle selected property

        // Spawn corresponding vehicle
        int playerNumber = (int)PhotonNetwork.player.CustomProperties["PlayerNumber"];
        Vector3 pos = spawnPoints[playerNumber].spawnPoint;
        Quaternion rot = spawnPoints[playerNumber].spawnRotation;

        FindObjectOfType<UISoundControl>().BGM.setParameterValue("Stage", 1.0f);

        GameObject mainCamera = Instantiate(cam);

        NetworkManager.localPlayer = PhotonNetwork.Instantiate("War_Buggy", pos, rot, 0);
        ((NetworkPlayerData)NetworkManager.localPlayer.GetComponent(typeof(NetworkPlayerData))).RegisterSpawnInformation(pos, rot);
        ((NetworkPlayerVisual)NetworkManager.localPlayer.GetComponent(typeof(NetworkPlayerVisual))).InitializeVehicleWithPlayerColor();

        mainCamera.GetComponent<CameraControl>().target = NetworkManager.localPlayer;
        NetworkManager.playerCamera = mainCamera.transform.Find("Camera").GetComponent<Camera>();

        photonView.RPC("RpcPlayerSpawnedHandler", PhotonTargets.All, NetworkManager.localPlayer.GetPhotonView().ownerId);
    }

    #region Photon RPC Callers
    [PunRPC]
    public void RpcPlayerJoinedSceneHandler(int playerId)
    {
        playersInScene[playerId] = true;

        // Master client will start the match
        if (PhotonNetwork.isMasterClient)
        {
            // Loop through all entries to make sure every players are ready
            foreach(KeyValuePair<int, bool> entry in playersInScene)
            {
                if (!entry.Value)
                {
                    goto playerNotReady;
                }
            }

            // Once all players have joined the scene, start spawning
            photonView.RPC("RpcRoundStartHandler", PhotonTargets.All);
        }

        // GOTO label if any of the player is not ready
        playerNotReady:
        return;
    }

    [PunRPC]
    public void RpcRoundStartHandler()
    {
        // Spawn players
        SpawnPlayer();

        // Spawn power ups
        PowerUpSpawnManager.instance.SpawnPowerUp();

        // Enable InGame HUD
        InGameHUDManager.instance.EnableInGameHUD();
    }

    [PunRPC]
    public void RpcPlayerSpawnedHandler(int playerId)
    {
        // All clients will keep a copy of this data
        playersStillAlive[playerId] = true;

        // Calls individual arrow indicators to find spawned player object and start tracking
    }

    [PunRPC]
    public void RpcPlayerDeathHandler(int playerId)
    {
        playersStillAlive[playerId] = false;

        // Only master client will execute following logics
        if (PhotonNetwork.isMasterClient)
        {
            uint aliveCount = 0;
            foreach (KeyValuePair<int, bool> entry in playersStillAlive)
            {
                if (entry.Value)
                {
                    ++aliveCount;
                }
            }

            if (aliveCount <= 1)
            {
                RoundOver(0);
            }
        }
    }

    private void RoundOver(int winnerID)
    {
        // Load end of match scene
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.LoadLevel("MatchResult");
        }
    }
    #endregion

    #region Local Gameplay

    public void SpawnLocalPlayers(string playerPrefabName, int numberOfPlayers)
    {
        lobbyUI.SetActive(false);
        // Loop every number of player necessary to create object
        for (int i = 0; i < numberOfPlayers; ++i)
        {
            // Spawn the vehicle based on spawnpoint index's position and rotation
            GameObject car = PhotonNetwork.Instantiate(playerPrefabName, spawnPoints[i].spawnPoint, spawnPoints[i].spawnRotation, 0);

            // Register the spawn position and rotation for future respawn
            car.GetComponent<NetworkPlayerData>().RegisterSpawnInformation(spawnPoints[i].spawnPoint, spawnPoints[i].spawnRotation);

            // Spawn camera control object to follow the vehicle
            GameObject cameraControl = Instantiate(cam, spawnPoints[i].spawnPoint, spawnPoints[i].spawnRotation);

            // Assign such control with target to follow
            cameraControl.GetComponent<CameraControl>().target = car;

            // For split screen mode
            if (numberOfPlayers > 1)
            {
                // Assign seperate control index to the vehicle
                car.GetComponent<CarUserControl>().playerNum = i + 1;

                // Calculate the Camera division
                float marginX = (i > 1) ? (i - 2) * 0.5f : i * 0.5f;
                float marginY;
                if (numberOfPlayers == 2)
                {
                    marginY = (i > 1) ? 0.5f : 0f;
                }
                else
                {
                    marginY = (i > 1) ? 0f : 0.5f;
                }
                cameraControl.GetComponentInChildren<Camera>().rect = new Rect(marginX, marginY, 0.5f, (numberOfPlayers == 2) ? 1f : 0.5f);
                car.GetComponent<NetworkPlayerData>().localCam = cameraControl.GetComponentInChildren<Camera>();
            }
            car.GetComponent<NetworkPlayerVisual>().InitializeVehicleWithPlayerColor();
        }
    }
    #endregion

    public void TransitionToLobby(string name, int num, int winnerID = -2)
    {
        if (NetworkManager.offlineMode)
        {
            playerPrefabName = name;
            numOfLocalPlayers = num;
        }
    }

    public void DestroyPlayerObject()
    {
        if (NetworkManager.localPlayer != null && NetworkManager.playerCamera != null)
        {
            //PhotonNetwork.DestroyPlayerObjects(PhotonPlayer.Find(NetworkManager.localPlayer.GetPhotonView().ownerId));
            PhotonNetwork.Destroy(NetworkManager.localPlayer);
            Destroy(NetworkManager.playerCamera.transform.root.gameObject);
            NetworkManager.localPlayer = null;
            NetworkManager.playerCamera = null;
        }
    }
}

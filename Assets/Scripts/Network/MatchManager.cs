using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Author: Jason Lin
 * 
 * Description:
 * MatchManager controls a match round's flow, from taking care of correct sequence of spawning to handling player's death across network/local split screen.
 */
public class MatchManager : Photon.MonoBehaviour
{
    /// <summary>
    /// Player camera prefab that will be instantiated during spawning player
    /// </summary>
    [SerializeField]
    private GameObject playerCameraPrefab;

    /// <summary>
    /// Player spawn point pre-mapped in scene
    /// </summary>
    private NetworkSpawnPoint[] spawnPoints;

    /// <summary>
    /// Dictionary data to keep track of which player is still alive, using player's photon ID / local player num as key
    /// </summary>
    private Dictionary<int, bool> playersStillAlive;

    /// <summary>
    /// Dictionary data to keep track of player across network has finished loading the scene, using player's photon ID as key
    /// </summary>
    private Dictionary<int, bool> playersInScene;

    /// <summary>
    /// Default player vehicle prefab name
    /// </summary>
    private string playerPrefabName = "War_Buggy";

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
        if (!playerCameraPrefab)
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

        GameObject mainCamera = Instantiate(playerCameraPrefab);

        GameObject playerGO = PhotonNetwork.Instantiate("War_Buggy", pos, rot, 0);
        ((NetworkPlayerData)playerGO.GetComponent(typeof(NetworkPlayerData))).RegisterSpawnInformation(pos, rot);
        ((NetworkPlayerVisual)playerGO.GetComponent(typeof(NetworkPlayerVisual))).InitializeVehicleWithPlayerColor();

        mainCamera.GetComponent<CameraControl>().target = playerGO;
        Camera playerCam = mainCamera.transform.Find("Camera").GetComponent<Camera>();

        NetworkManager.instance.SetLocalPlayer(playerGO, playerCam);

        photonView.RPC("RpcPlayerSpawnedHandler", PhotonTargets.All, playerGO.GetPhotonView().ownerId);
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
        // Calls the arrow indicator system to add the new player to list
        NetworkManager.instance.GetLocalPlayer().GetComponent<ArrowIndicationSystem>().UpdateArrowList();
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
        // Loop every number of player necessary to create object
        for (int i = 0; i < numberOfPlayers; ++i)
        {
            // Spawn the vehicle based on spawnpoint index's position and rotation
            GameObject car = PhotonNetwork.Instantiate(playerPrefabName, spawnPoints[i].spawnPoint, spawnPoints[i].spawnRotation, 0);

            // Register the spawn position and rotation for future respawn
            car.GetComponent<NetworkPlayerData>().RegisterSpawnInformation(spawnPoints[i].spawnPoint, spawnPoints[i].spawnRotation);

            // Spawn camera control object to follow the vehicle
            GameObject cameraControl = Instantiate(playerCameraPrefab, spawnPoints[i].spawnPoint, spawnPoints[i].spawnRotation);

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

    public void DestroyPlayerObject(int playerNum = 0)
    {
        if (NetworkManager.localPlayer[playerNum] != null && NetworkManager.playerCamera[playerNum] != null)
        {
            //PhotonNetwork.DestroyPlayerObjects(PhotonPlayer.Find(NetworkManager.localPlayer.GetPhotonView().ownerId));
            NetworkManager.instance.DestroyLocalPlayer(playerNum);
        }
    }
}

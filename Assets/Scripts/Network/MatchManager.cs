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
    
    private void Start()
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

    private void AdjustCamera(Camera playerCam, int playerNum)
    {
        // For split screen mode
        if (NetworkManager.instance.numberOfLocalPlayers > 1)
        {
            // Calculate the Camera division
            float marginX = (playerNum > 1) ? (playerNum - 2) * 0.5f : playerNum * 0.5f;
            float marginY;
            if (NetworkManager.instance.numberOfLocalPlayers == 2)
            {
                marginY = (playerNum > 1) ? 0.5f : 0f;
            }
            else
            {
                marginY = (playerNum > 1) ? 0f : 0.5f;
            }
            playerCam.rect = new Rect(marginX, marginY, 0.5f, (NetworkManager.instance.numberOfLocalPlayers == 2) ? 1f : 0.5f);
        }
    }

    public void SpawnPlayer(int playerId)
    {
        // Obtain the player selected vehicle model
        string vehicleName = "War_Buggy";

        // Player's number is indicated by the loop counter in offline mode or the custom property in online mode
        int playerNumber = (NetworkManager.offlineMode) ? playerId : ((int)PhotonNetwork.player.CustomProperties["PlayerNumber"]);

        // Obtain the spawn position and rotation
        Vector3 pos = spawnPoints[playerNumber].spawnPoint;
        Quaternion rot = spawnPoints[playerNumber].spawnRotation;

        // Spawn player camera
        GameObject mainCamera = Instantiate(playerCameraPrefab);


        // Spawn player gameobject and register the spawn position and rotation for future use
        GameObject playerGO = PhotonNetwork.Instantiate(vehicleName, pos, rot, 0);
        playerGO.GetComponent<CarUserControl>().playerNum = playerId + 1;
        ((NetworkPlayerData)playerGO.GetComponent(typeof(NetworkPlayerData))).RegisterSpawnInformation(pos, rot);
        ((NetworkPlayerVisual)playerGO.GetComponent(typeof(NetworkPlayerVisual))).InitializeVehicleWithPlayerColor();

        // Assign the player camera to follow the corresponding player
        mainCamera.GetComponent<CameraControl>().target = playerGO;
        Camera playerCam = mainCamera.transform.Find("Camera").GetComponent<Camera>();
        playerCam.cullingMask = ~(1 << 8+playerNumber);

        // Offline mode requires additional adjustment
        if (NetworkManager.offlineMode)
        {
            // Adjust the camera viewport according to player number
            AdjustCamera(playerCam, playerId);

            // Change photonview id
            playerGO.GetPhotonView().ownerId = playerId + 1;

            playerCam.cullingMask = ~(1 << 8 + playerId);

        }

        // Set the local player reference
        NetworkManager.instance.SetLocalPlayer(playerGO, playerCam, (NetworkManager.offlineMode) ? playerId : 0);
        
    }

    private void SpawnPlayer()
    {
        // Loop condition value, offline mode is the number of local players in game, online mode is always 1
        int playersInGame = (NetworkManager.offlineMode) ? NetworkManager.instance.numberOfLocalPlayers : 1;

        // Loop to create n number of players to game
        for (int i = 0; i < playersInGame; ++i)
        {
            SpawnPlayer(i);
        }

        // Offline mode will use loop to register to arrow indicator
        if (NetworkManager.offlineMode)
        {
            foreach (GameObject player in NetworkManager.localPlayer)
            {
                player.GetComponent<ArrowIndicationSystem>().UpdateArrowList();
            }
        }
        // Online mode will use RPC to register to arrow indicator
        else
        {
            photonView.RPC("RpcPlayerSpawnedHandler", PhotonTargets.All, NetworkManager.instance.GetLocalPlayer().GetPhotonView().ownerId);
        }

        // Switch BGM
        FindObjectOfType<UISoundControl>().BGM.setParameterValue("Stage", 1.0f);
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
                MatchRoundOver();
            }
        }
    }

    public void MatchRoundOver()
    {
        // Load end of match scene
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.LoadLevel("MatchResult");
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

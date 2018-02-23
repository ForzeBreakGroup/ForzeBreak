using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchManager : Photon.MonoBehaviour
{
    public GameObject cam;
    private NetworkSpawnPoint[] spawnPoints;
    private Dictionary<PhotonPlayer, bool> playersStillAlive;
    private static MatchManager matchManager;
    private Dictionary<PhotonPlayer, bool> playersReady;
    private GameObject lobbyUI;
    private string playerPrefabName = "War_Buggy";
    private int numOfLocalPlayers = 0;
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
        playersStillAlive = new Dictionary<PhotonPlayer, bool>();
        playersReady = new Dictionary<PhotonPlayer, bool>();
        spawnPoints = FindObjectsOfType<NetworkSpawnPoint>();
        if (!cam)
        {
            Debug.LogError("Camera Not Attached");
        }
    }

    private void OnEnable()
    {
        // Network Event listener
        PhotonNetwork.OnEventCall += EvtPlayerDeathHandler;
        PhotonNetwork.OnEventCall += EvtAddPlayerToMatchHandler;
        PhotonNetwork.OnEventCall += EvtRemovePlayerFromMatchHandler;
        PhotonNetwork.OnEventCall += EvtPlayerReadyHandler;
        PhotonNetwork.OnEventCall += EvtSpawnPlayerHandler;
        PhotonNetwork.OnEventCall += EvtRoundOverHandler;

        EventManager.StartListening("OnPlayerReady", EvtPlayerReadyHandler);
    }

    private void OnDisable()
    {
        // Network Event listener
        PhotonNetwork.OnEventCall -= EvtPlayerDeathHandler;
        PhotonNetwork.OnEventCall -= EvtAddPlayerToMatchHandler;
        PhotonNetwork.OnEventCall -= EvtRemovePlayerFromMatchHandler;
        PhotonNetwork.OnEventCall -= EvtPlayerReadyHandler;
        PhotonNetwork.OnEventCall -= EvtSpawnPlayerHandler;
        PhotonNetwork.OnEventCall -= EvtRoundOverHandler;
    }

    #region Event Handlers
    private void EvtAddPlayerToMatchHandler(byte evtCode, object content, int senderid)
    {
        if (evtCode == (byte)ENetworkEventCode.OnAddPlayerToMatch && PhotonNetwork.isMasterClient)
        {
            PhotonPlayer newPlayer = (PhotonPlayer)content;

            if (!playersStillAlive.ContainsKey(newPlayer))
            {
                playersStillAlive.Add(newPlayer, true);
            }
        }
    }

    private void EvtRemovePlayerFromMatchHandler(byte evtCode, object content, int senderid)
    {
        if (evtCode == (byte)ENetworkEventCode.OnRemovePlayerFromMatch && PhotonNetwork.isMasterClient)
        {
            Debug.Log("EvtRemovePlayerEvent");
            PhotonPlayer otherPlayer = (PhotonPlayer)content;

            if (playersStillAlive.ContainsKey(otherPlayer))
            {
                playersStillAlive.Remove(otherPlayer);
            }
        }
    }

    private void EvtPlayerDeathHandler(byte evtCode, object content, int senderid)
    {
        if (evtCode == (byte)ENetworkEventCode.OnPlayerDeath && PhotonNetwork.isMasterClient)
        {
            Debug.Log("Player Death Event Handler");
            PhotonPlayer sender = PhotonPlayer.Find(senderid);
            playersStillAlive[sender] = false;

            uint aliveCount = 0;
            PhotonPlayer surviver;
            foreach(KeyValuePair<PhotonPlayer, bool> entry in playersStillAlive)
            {
                if (entry.Value)
                {
                    ++aliveCount;
                    surviver = entry.Key;
                }
            }

            if (aliveCount <= 1)
            {
                RoundOver();
            }
        }
    }

    private void RoundOver()
    {
        List<PhotonPlayer> keyEntry = new List<PhotonPlayer>(playersReady.Keys);

        foreach(PhotonPlayer player in keyEntry)
        {
            playersReady[player] = false;
        }

        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.All;
        PhotonNetwork.RaiseEvent((byte)ENetworkEventCode.OnRoundOver, null, true, options);
    }

    private void EvtPlayerReadyHandler(byte evtCode, object content, int senderid)
    {
        if (evtCode == (byte)ENetworkEventCode.OnPlayerReady && PhotonNetwork.isMasterClient)
        {
            PhotonPlayer sender = PhotonPlayer.Find(senderid);
            playersReady[sender] = (bool)content;

            int numOfReady = 0;
            foreach (KeyValuePair<PhotonPlayer, bool> entry in playersReady)
            {
                if (entry.Value)
                {
                    ++numOfReady;
                }
            }

            if (numOfReady == PhotonNetwork.playerList.Length && PhotonNetwork.playerList.Length > 1)
            {
                RaiseEventOptions options = new RaiseEventOptions();
                options.Receivers = ReceiverGroup.All;
                PhotonNetwork.RaiseEvent((byte)ENetworkEventCode.OnPlayerSpawning, null, true, options);
            }
        }
    }

    private void EvtPlayerReadyHandler()
    {
        SpawnLocalPlayers(playerPrefabName, numOfLocalPlayers); 
    }

    private void EvtSpawnPlayerHandler(byte evtCode, object content, int senderid)
    {
        if (evtCode == (byte)ENetworkEventCode.OnPlayerSpawning)
        {
            lobbyUI.SetActive(false);

            int playerNumber = (int)PhotonNetwork.player.CustomProperties["PlayerNumber"];
            Vector3 pos = spawnPoints[playerNumber].spawnPoint;
            Quaternion rot = spawnPoints[playerNumber].spawnRotation;

            NetworkManager.localPlayer = PhotonNetwork.Instantiate("War_Buggy", pos, rot, 0);
            ((NetworkPlayerData)NetworkManager.localPlayer.GetComponent(typeof(NetworkPlayerData))).RegisterSpawnInformation(pos, rot);
            ((NetworkPlayerVisual)NetworkManager.localPlayer.GetComponent(typeof(NetworkPlayerVisual))).InitializeVehicleWithPlayerColor();

            GameObject mainCamera = Instantiate(cam);
            mainCamera.GetComponent<CameraControl>().target = NetworkManager.localPlayer;
            NetworkManager.playerCamera = mainCamera.transform.Find("Camera").GetComponent<Camera>();

            RaiseEventOptions options = new RaiseEventOptions();
            options.Receivers = ReceiverGroup.All;
            PhotonNetwork.RaiseEvent((byte)ENetworkEventCode.OnPlayerSpawnFinished, null, true, options);
        }
    }

    private void EvtRoundOverHandler(byte evtCode, object content, int senderid)
    {
        if (evtCode == (byte) ENetworkEventCode.OnRoundOver)
        {
            DestroyPlayerObject();
            MatchManager.instance.TransitionToLobby(playerPrefabName, numOfLocalPlayers);
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

    public void TransitionToLobby(string name, int num)
    {
        if (NetworkManager.offlineMode)
        {
            playerPrefabName = name;
            numOfLocalPlayers = num;
        }
        lobbyUI.SetActive(true);
    }

    public void DestroyPlayerObject()
    {
        if (NetworkManager.localPlayer != null && NetworkManager.playerCamera != null)
        {
            PhotonNetwork.DestroyPlayerObjects(PhotonPlayer.Find(NetworkManager.localPlayer.GetPhotonView().ownerId));
            Destroy(NetworkManager.playerCamera.transform.root.gameObject);
            NetworkManager.localPlayer = null;
            NetworkManager.playerCamera = null;
        }
    }
}

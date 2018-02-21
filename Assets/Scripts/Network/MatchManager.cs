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
    private bool isReady = false;
    private GameObject lobbyUI;
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
        playersStillAlive = new Dictionary<PhotonPlayer, bool>();
        playersReady = new Dictionary<PhotonPlayer, bool>();
        spawnPoints = FindObjectsOfType<NetworkSpawnPoint>();
        lobbyUI = FindObjectOfType<Canvas>().transform.Find("Lobby").gameObject;
        if (!cam)
        {
            Debug.LogError("Camera Not Attached");
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.OnEventCall += EvtPlayerDeathHandler;
        PhotonNetwork.OnEventCall += EvtAddPlayerToMatchHandler;
        PhotonNetwork.OnEventCall += EvtRemovePlayerFromMatchHandler;
        PhotonNetwork.OnEventCall += EvtPlayerReadyHandler;
        PhotonNetwork.OnEventCall += EvtSpawnPlayerHandler;
    }

    private void OnDisable()
    {
        PhotonNetwork.OnEventCall -= EvtPlayerDeathHandler;
        PhotonNetwork.OnEventCall -= EvtAddPlayerToMatchHandler;
        PhotonNetwork.OnEventCall -= EvtRemovePlayerFromMatchHandler;
        PhotonNetwork.OnEventCall -= EvtPlayerReadyHandler;
        PhotonNetwork.OnEventCall += EvtSpawnPlayerHandler;
    }

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
        if (evtCode == (byte) ENetworkEventCode.OnPlayerDeath && PhotonNetwork.isMasterClient)
        {
            Debug.Log("Player Death Event Handler");
            PhotonPlayer sender = PhotonPlayer.Find(senderid);
            playersStillAlive[sender] = false;

            uint aliveCount = 0;
            foreach(KeyValuePair<PhotonPlayer, bool> entry in playersStillAlive)
            {
                if (entry.Value)
                {
                    ++aliveCount;
                }
            }

            if (aliveCount <= 1)
            {
                PhotonNetwork.LoadLevel("End");
            }
        }
    }

    private void EvtPlayerReadyHandler(byte evtCode, object content, int senderid)
    {
        if (evtCode == (byte) ENetworkEventCode.OnPlayerReady && PhotonNetwork.isMasterClient)
        {
            Debug.Log("Player is Ready");
            PhotonPlayer sender = PhotonPlayer.Find(senderid);
            playersReady[sender] = (bool)content;

            int numOfReady = 0;
            foreach(KeyValuePair<PhotonPlayer, bool> entry in playersReady)
            {
                if (entry.Value)
                {
                    ++numOfReady;
                }
            }
            Debug.Log(numOfReady);
            Debug.Log(PhotonNetwork.playerList.Length);

            if (numOfReady == PhotonNetwork.playerList.Length)
            {
                RaiseEventOptions options = new RaiseEventOptions();
                options.Receivers = ReceiverGroup.All;
                PhotonNetwork.RaiseEvent((byte)ENetworkEventCode.OnPlayerSpawning, null, true, options);
            }
        }
    }

    private void EvtSpawnPlayerHandler(byte evtCode, object content, int senderid)
    {
        if (evtCode == (byte) ENetworkEventCode.OnPlayerSpawning)
        {
            Debug.Log("Spawning");
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

    public void SpawnLocalPlayers(string playerPrefabName, int numberOfPlayers)
    {
        foreach (string name in Input.GetJoystickNames())
        {
            Debug.Log(name);
        }

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
            }
        }
    }

    public void ReadyState()
    {
        isReady = !isReady;
        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.MasterClient;
        PhotonNetwork.RaiseEvent((byte)ENetworkEventCode.OnPlayerReady, isReady, true, options);
    }
}

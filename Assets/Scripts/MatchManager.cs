using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : Photon.MonoBehaviour
{
    public GameObject cam;
    private NetworkSpawnPoint[] spawnPoints;
    private Dictionary<PhotonPlayer, bool> playersStillAlive;
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
        spawnPoints = FindObjectsOfType<NetworkSpawnPoint>();
    }

    private void Awake()
    {
        playersStillAlive = new Dictionary<PhotonPlayer, bool>();
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
    }

    private void OnDisable()
    {
        PhotonNetwork.OnEventCall -= EvtPlayerDeathHandler;
        PhotonNetwork.OnEventCall -= EvtAddPlayerToMatchHandler;
        PhotonNetwork.OnEventCall -= EvtRemovePlayerFromMatchHandler;
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

            if (!playersStillAlive.ContainsKey(otherPlayer))
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

    public void SpawnPlayer(string playerPrefabName)
    {
        int playerCount = PhotonNetwork.playerList.Length;
        Vector3 pos = spawnPoints[playerCount % spawnPoints.Length].spawnPoint;
        Quaternion rot = spawnPoints[playerCount % spawnPoints.Length].spawnRotation;

        NetworkManager.localPlayer = PhotonNetwork.Instantiate(playerPrefabName, pos, rot, 0);
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

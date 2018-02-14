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

        switch (numberOfPlayers)
        {
            case 1:
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    GameObject car = PhotonNetwork.Instantiate(playerPrefabName, spawnPoints[i].spawnPoint, spawnPoints[i].spawnRotation, 0);
                    car.GetComponent<NetworkPlayerData>().RegisterSpawnInformation(spawnPoints[i].spawnPoint, spawnPoints[i].spawnRotation);
                    GameObject go = Instantiate(cam, spawnPoints[i].spawnPoint, spawnPoints[i].spawnRotation);
                    go.GetComponent<CameraControl>().target = car;
                }
                break;

            case 2:
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    GameObject car = PhotonNetwork.Instantiate(playerPrefabName, spawnPoints[i].spawnPoint, spawnPoints[i].spawnRotation, 0);
                    car.GetComponent<CarUserControl>().playerNum = i + 1;
                    car.GetComponent<NetworkPlayerData>().RegisterSpawnInformation(spawnPoints[i].spawnPoint, spawnPoints[i].spawnRotation);
                    GameObject go = Instantiate(cam, spawnPoints[i].spawnPoint, spawnPoints[i].spawnRotation);
                    go.GetComponent<CameraControl>().target = car;

                    float marginX = (i > 1) ? (i - 2) * 0.5f : i * 0.5f;
                    float marginY = (i > 1) ? 0.5f : 0f;

                    go.GetComponentInChildren<Camera>().rect = new Rect(marginX, marginY, 0.5f, 1f);

                }
                break;
            default:
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    GameObject car = PhotonNetwork.Instantiate(playerPrefabName, spawnPoints[i].spawnPoint, spawnPoints[i].spawnRotation, 0);
                    car.GetComponent<CarUserControl>().playerNum = i + 1;
                    car.GetComponent<NetworkPlayerData>().RegisterSpawnInformation(spawnPoints[i].spawnPoint, spawnPoints[i].spawnRotation);
                    GameObject go = Instantiate(cam, spawnPoints[i].spawnPoint, spawnPoints[i].spawnRotation);
                    go.GetComponent<CameraControl>().target = car;

                    float marginX = (i > 1) ? (i - 2) * 0.5f : i * 0.5f;
                    float marginY = (i > 1) ? 0f : 0.5f;

                    go.GetComponentInChildren<Camera>().rect = new Rect(marginX, marginY, 0.5f, 0.5f);

                }
                break;

        }
    }

    public void SpawnPlayer(string playerPrefabName, bool offlineMode = false)
    {
        int playerCount = PhotonNetwork.countOfPlayers;
        Vector3 pos = spawnPoints[playerCount % spawnPoints.Length].spawnPoint;
        Quaternion rot = spawnPoints[playerCount % spawnPoints.Length].spawnRotation;

        NetworkManager.localPlayer = PhotonNetwork.Instantiate(playerPrefabName, pos, rot, 0);
        ((NetworkPlayerData)NetworkManager.localPlayer.GetComponent(typeof(NetworkPlayerData))).RegisterSpawnInformation(pos, rot);

        GameObject go = Instantiate(cam);
        go.GetComponent<CameraControl>().target = NetworkManager.localPlayer;
    }
}

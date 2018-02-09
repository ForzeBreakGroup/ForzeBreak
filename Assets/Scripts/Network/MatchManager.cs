using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : Photon.MonoBehaviour
{
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
            Debug.Log("EvtAddPlayerEvent");
            PhotonPlayer player = PhotonPlayer.Find(senderid);

            if (!playersStillAlive.ContainsKey(player))
            {
                playersStillAlive.Add(player, true);
            }
        }
    }

    private void EvtRemovePlayerFromMatchHandler(byte evtCode, object content, int senderid)
    {
        if (evtCode == (byte)ENetworkEventCode.OnRemovePlayerFromMatch && PhotonNetwork.isMasterClient)
        {
            Debug.Log("EvtRemovePlayerEvent");
            PhotonPlayer player = PhotonPlayer.Find(senderid);

            if (!playersStillAlive.ContainsKey(player))
            {
                playersStillAlive.Remove(player);
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

    public void SpawnPlayer(string playerPrefabName)
    {
        int playerCount = PhotonNetwork.countOfPlayers;
        Vector3 pos = spawnPoints[playerCount % spawnPoints.Length].spawnPoint;
        Quaternion rot = spawnPoints[playerCount % spawnPoints.Length].spawnRotation;
        NetworkManager.localPlayer = PhotonNetwork.Instantiate(playerPrefabName, pos, rot, 0);
        ((NetworkPlayerData)NetworkManager.localPlayer.GetComponent(typeof(NetworkPlayerData))).RegisterSpawnInformation(pos, rot);
    }
}

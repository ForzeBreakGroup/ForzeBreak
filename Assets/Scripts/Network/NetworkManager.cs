using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;

/*
 * Author: Jason Lin
 * 
 * Description:
 * NetworkManager for handling Photon server related calls as well as exposing related methods for create/join room
 * 
 */
public class NetworkManager : PunBehaviour
{
    #region Public Members
    public bool isLocalTesting = false;
    public string gameVersion = "0.1.0";
    public static GameObject localPlayer;
    public static NetworkManager instance
    {
        get
        {
            if (!networkManager)
            {
                networkManager = FindObjectOfType<NetworkManager>() as NetworkManager;

                if (!networkManager)
                {
                    Debug.LogError("NetworkManager must be attached to a gameobject in scene");
                }
                else
                {
                    networkManager.Init();
                }
            }

            return networkManager;
        }
    }
    #endregion

    #region Private Members
    private static NetworkManager networkManager;
    [SerializeField] private string playerPrefabName = "Player";
    [SerializeField] private string onlineSceneName = "Arena1";

    private List<NetworkSpawnPoint> spawnPositions;
    private int spawnPoint = 0;
    #endregion

    #region Public Methods
    public void CreateGame()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 4;
        roomOptions.PlayerTtl = 7500;
        roomOptions.EmptyRoomTtl = 1000;
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();

        PhotonNetwork.CreateRoom(null, roomOptions, null);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    #endregion

    #region Private Methods
    private void Start()
    {
        SceneManager.sceneLoaded += this.OnLevelLoaded;
    }

    private void Awake()
    {
        if (networkManager)
        {
            DestroyImmediate(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        networkManager = this;
        networkManager.Init();
    }

    private void Init()
    {
        if (!PhotonNetwork.ConnectUsingSettings(gameVersion))
        {
            Debug.LogError("Connecting to Photon Server Failed");
        }
        else
        {
            // Configure PhotonNetwork settings
            PhotonNetwork.automaticallySyncScene = true;
            PhotonNetwork.sendRate = 20;
            PhotonNetwork.sendRateOnSerialize = 20;
        }
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        Debug.Log("Scene Loaded");

        // Host will handle all the client spawning including providing spawn location and rotation
        if (PhotonNetwork.isMasterClient)
        {
            RegisterSpawnLocations();
            Vector3 pos;
            Quaternion rot;
            GetSpawnPoint(out pos, out rot);
            RPCSpawnPlayer(pos, rot);
        }
    }

    [PunRPC]
    private void RPCSpawnPlayer(Vector3 position, Quaternion rotation)
    {
        if (!PhotonNetwork.inRoom)
            return;

        localPlayer = PhotonNetwork.Instantiate(playerPrefabName, position, rotation, 0);
        ((NetworkPlayerData)localPlayer.GetComponent(typeof(NetworkPlayerData))).RegisterSpawnInformation(position, rotation);

        RaiseEventOptions evtOptions = new RaiseEventOptions();
        evtOptions.Receivers = ReceiverGroup.MasterClient;
        PhotonNetwork.RaiseEvent((byte)ENetworkEventCode.OnAddPlayerToMatch, null, true, evtOptions);
    }

    private void GetSpawnPoint(out Vector3 position, out Quaternion rotation)
    {
        // Validate spawn position has content
        if (spawnPositions == null || spawnPositions.Count == 0)
        {
            // Find the spawn locations if spawn positions are invalid
            RegisterSpawnLocations();

            // If spawn positions still invalid, return Vector3.zero
            if (spawnPositions == null || spawnPositions.Count == 0)
            {
                position = Vector3.zero;
                rotation = Quaternion.identity;
            }
        }
        position = spawnPositions[spawnPoint].spawnPoint;
        rotation = spawnPositions[spawnPoint].spawnRotation;
        spawnPoint = ++spawnPoint % spawnPositions.Count;
    }

    private void RegisterSpawnLocations()
    {
        spawnPoint = 0;
        spawnPositions = new List<NetworkSpawnPoint>();
        NetworkSpawnPoint[] spawnPoints = FindObjectsOfType<NetworkSpawnPoint>();
        foreach (NetworkSpawnPoint sp in spawnPoints)
        {
            spawnPositions.Add(sp);
        }
    }
    #endregion

    #region Photon SDK Overrides
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        base.OnJoinedRoom();

        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.LoadLevel(onlineSceneName);
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left Room");
        base.OnLeftRoom();

        RaiseEventOptions evtOptions = new RaiseEventOptions();
        evtOptions.Receivers = ReceiverGroup.MasterClient;
        PhotonNetwork.RaiseEvent((byte)ENetworkEventCode.OnRemovePlayerFromMatch, null, true, evtOptions);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        base.OnCreatedRoom();
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        Debug.LogError("Error Code: " + codeAndMsg[0] + ", " + codeAndMsg[1]);
        base.OnPhotonJoinRoomFailed(codeAndMsg);
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.LogError("Error Code: " + codeAndMsg[0] + ", " + codeAndMsg[1]);
        base.OnPhotonRandomJoinFailed(codeAndMsg);
    }

    public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        Debug.LogError("Error Code: " + codeAndMsg[0] + ", " + codeAndMsg[1]);
        base.OnPhotonCreateRoomFailed(codeAndMsg);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("JoinedLobby");
        base.OnJoinedLobby();

        if (isLocalTesting)
        {
            PhotonNetwork.JoinOrCreateRoom("Testing", new RoomOptions(), null);
        }
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log("Player Joined: " + newPlayer.ID);
        base.OnPhotonPlayerConnected(newPlayer);

        if (PhotonNetwork.isMasterClient)
        {
            Vector3 pos;
            Quaternion rot;
            GetSpawnPoint(out pos, out rot);
            photonView.RPC("RPCSpawnPlayer", newPlayer, pos, rot);
        }
    }
    #endregion
}

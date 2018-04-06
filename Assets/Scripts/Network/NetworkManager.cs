using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;

/*
 * Author: Jason Lin
 * 
 * Description:
 * NetworkManager for handling Photon service related calls as well as exposing related methods for create/join room
 * for the UI elements.
 * 
 * By default, NetworkManager is not connected to the PhotonService. Once the player choose to CREATE or JOIN room
 * then it will start the connection
 * 
 * Remarks:
 * The default code flow of the program.
 * Online Mode -> Connect to Photon Server -> OnJoinMaster callback -> Join Random Room -> Join Random Room Failed (No room available) -> Create Room
 */
public class NetworkManager : PunBehaviour
{
    #region Public Members
    /// <summary>
    /// Boolean indicates if the NetworkManager will be started in Offline Mode - Not connected to server, using split screen by default
    /// </summary>
    public static bool offlineMode = false;

    /// <summary>
    /// Game version used by Photon service to disjoint different game versions from cross playing with each other
    /// </summary>
    public string gameVersion = "0.1.0";

    /// <summary>
    /// A static reference to the player game object used by this remote client
    /// </summary>
    public static GameObject[] localPlayer;

    /// <summary>
    /// A static reference to the player camera used by this remote client
    /// </summary>
    public static Camera[] playerCamera;

    /// <summary>
    /// For local split screen usage, total number of players for split screen
    /// </summary>
    [Range(1, 4)]
    [SerializeField]
    public int numberOfLocalPlayers = 1;

    /// <summary>
    /// A static global reference to NetworkManager, creating an instance for singleton access pattern
    /// </summary>
    public static NetworkManager instance
    {
        get
        {
            // If NetworkManager has not been initialized before, it's likely that it's first loadup
            if (!networkManager)
            {
                // Try to find an object in current scene with NetworkManager script
                networkManager = FindObjectOfType<NetworkManager>() as NetworkManager;

                // If still can't find it, throw an error
                if (!networkManager)
                {
                    Debug.LogError("NetworkManager must be attached to a gameobject in scene");
                }

                // Otherwise, initialize it
                else
                {
                    networkManager.Init();
                }
            }

            // Return the internal reference
            return networkManager;
        }
    }
    #endregion

    #region Private Members
    /// <summary>
    /// Internal static reference of NetworkManager for instance usage
    /// </summary>
    private static NetworkManager networkManager;

    /// <summary>
    /// The player gameobject prefab name string, the prefab must be under /Resources/
    /// </summary>
    [SerializeField] private string playerPrefabName = "Player";

    /// <summary>
    /// The default online session scene
    /// </summary>
    [SerializeField] private string onlineSceneName = "Arena1";

    /// <summary>
    /// A list of spawn posisions obtained from scene
    /// </summary>
    private List<NetworkSpawnPoint> spawnPositions;

    [SerializeField]
    Color[] playerColors = new Color[] { Color.blue, Color.red, Color.green, Color.yellow };
    #endregion

    #region Public Interface Methods
    public void StartMatchMaking()
    {
        if (!PhotonNetwork.connected)
        {
            ConnectingToPhotonServer();
        }
        OnConnectedToMaster();
    }

    public void StartSplitScreen(int numOfPlayer = 4)
    {
        this.numberOfLocalPlayers = numOfPlayer;

        if (PhotonNetwork.connected)
        {
            DisconnectFromPhoton();
        }
    }

    public Room[] RefreshCustomRoomList()
    {
        return null;
    }

    public void JoinRoomByName(string name)
    {

    }

    public void ChangePlayerName(string name = "Player")
    {
        PhotonNetwork.playerName = name;
    }

    public Color GetPlayerColor(int index)
    {
        return playerColors[index];
    }

    public void EnableTheRoom(bool enable)
    {
        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.room.IsVisible = enable;
            PhotonNetwork.room.IsOpen = enable;
        }
    }

    public GameObject GetLocalPlayer(int playerNum = 0)
    {
        return localPlayer[playerNum];
    }

    public Camera GetPlayerCamera(int playerNum = 0)
    {
        return playerCamera[playerNum];
    }

    public void SetLocalPlayer(GameObject playerGO, Camera playerCam, int playerNum = 0)
    {
        localPlayer[playerNum] = playerGO;
        playerCamera[playerNum] = playerCam;
    }

    public void DestroyLocalPlayer(int playerNum = 0)
    {
        PhotonNetwork.Destroy(localPlayer[playerNum]);
        Destroy(playerCamera[playerNum].transform.root.gameObject);
        localPlayer[playerNum] = null;
        playerCamera[playerNum] = null;
    }

    public bool ValidateOwnership(PhotonView view, int playerNum = 0)
    {
        // Offline mode will use the playern
        if (offlineMode)
        {
            return (view.ownerId == playerNum);
        }
        else
        {
            return view.isMine;
        }
    }
    #endregion

    #region Private Methods
    private void Awake()
    {
        // If NetworkManager has already been initialized on Awake, destroy the most recent one
        if (networkManager)
        {
            DestroyImmediate(gameObject);
            return;
        }

        // Mark the NetworkManager as DontDestroyOnLoad
        DontDestroyOnLoad(gameObject);
        networkManager = this;
        networkManager.Init();

        ConnectingToPhotonServer();
    }

    /// <summary>
    /// Safely disconnect the PhotonServer when application is closing
    /// </summary>
    private void OnApplicationQuit()
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.offlineMode = false;
    }

    /// <summary>
    /// All initalization that is absolutely necessary after calls goes here
    /// </summary>
    private void Init()
    {
        localPlayer = new GameObject[numberOfLocalPlayers];
        playerCamera = new Camera[numberOfLocalPlayers];
    }

    /// <summary>
    /// Method to connect to photonServer with predefined settings
    /// </summary>
    private void ConnectingToPhotonServer()
    {
        // If connection failed, throw error
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

            // Default player nickname
            // ChangePlayerName();
        }
    }

    /// <summary>
    /// Create Game room in PhotonServer
    /// </summary>
    private void CreateRoomInPhotonServer()
    {
        // Default RoomOption
        // Open and Visible to all paries
        // Maximum of 4 players
        // Player's will be discarded after 7.5 seconds since disconnections to host
        // If the room is empty after 1 second, it will be cleared from PhotonServer
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 4;
        roomOptions.PlayerTtl = 7500;
        roomOptions.EmptyRoomTtl = 1000;

        PhotonNetwork.CreateRoom(null, roomOptions, null);
    }

    /// <summary>
    /// Randomly join a game assigned by photon server with same game version
    /// </summary>
    private void JoinRandomGameInPhotonServer()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary>
    /// Method to disconnect from photon server
    /// </summary>
    private void DisconnectFromPhoton()
    {
        offlineMode = true;
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.Disconnect();
        }
        PhotonNetwork.offlineMode = true;
    }

    private void SetPlayerCustomProperties()
    {
        // Set custom property to identify player's color
        ExitGames.Client.Photon.Hashtable playerInfo = new ExitGames.Client.Photon.Hashtable();

        // Unity Color cannot be serailized through photon, manual serializing it
        Color c = playerColors[PhotonNetwork.playerList.Length - 1];
        float[] serializedColor = new float[4];
        serializedColor[0] = c.r;
        serializedColor[1] = c.g;
        serializedColor[2] = c.b;
        serializedColor[3] = c.a;

        playerInfo.Add("Color", serializedColor);
        playerInfo.Add("PlayerNumber", (int)(PhotonNetwork.playerList.Length - 1));
        PhotonNetwork.player.SetCustomProperties(playerInfo);
    }
    #endregion

    #region Photon SDK Overrides
    /// <summary>
    /// Overrides the OnConnectedToMaster event, which is called when not connected to PhotonServer
    /// </summary>
    public override void OnConnectedToMaster()
    {
        Debug.Log("JoinedMaster");
        base.OnConnectedToMaster();

        if (offlineMode)
        {
            PhotonNetwork.CreateRoom("");
        }
        else
        {
            JoinRandomGameInPhotonServer();
        }
    }

    /// <summary>
    /// Overrides OnJoinRoom which is called when client joins a room (including host)
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        base.OnJoinedRoom();

        // Only needed in network mode
        if (PhotonNetwork.connected)
        {
            SetPlayerCustomProperties();
        }

        // The host will call the change scene
        if (PhotonNetwork.isMasterClient)
        {
            // Load new scene
            PhotonNetwork.LoadLevel(onlineSceneName);
        }
    }

    /// <summary>
    /// Override OnLeftRoom which is called when client left a room
    /// </summary>
    public override void OnLeftRoom()
    {
        Debug.Log("Left Room");
        base.OnLeftRoom();

        PhotonNetwork.LoadLevel("Menu");
    }

    /// <summary>
    /// Override OnCreateRoom, which is called when player is hosting a new room
    /// </summary>
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        base.OnCreatedRoom();
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        base.OnPhotonRandomJoinFailed(codeAndMsg);
        CreateRoomInPhotonServer();
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        Debug.Log("Player Disconnected: " + otherPlayer.ID);
        base.OnPhotonPlayerDisconnected(otherPlayer);
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log("Player Joined: " + newPlayer.ID);
        base.OnPhotonPlayerConnected(newPlayer);
    }

    public override void OnReceivedRoomListUpdate()
    {
        base.OnReceivedRoomListUpdate();

        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
    }
    #endregion
}

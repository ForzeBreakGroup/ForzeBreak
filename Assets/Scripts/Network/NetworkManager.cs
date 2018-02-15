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
    public static GameObject localPlayer;

    /// <summary>
    /// A static reference to the player camera used by this remote client
    /// </summary>
    public static Camera playerCamera;

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
    /// Enum defines connection state of the networkmanager
    /// </summary>
    private enum ConnectionState
    {
        /// <summary>
        /// NetworkManager IDLE for communication
        /// </summary>
        IDLE,

        /// <summary>
        /// NetworkManager received CREATE room command from user
        /// </summary>
        CREATE,

        /// <summary>
        /// NetworkManager received JOIN room coomand from user
        /// </summary>
        JOIN
    };

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

    /// <summary>
    /// For local split screen usage, total number of players for split screen
    /// </summary>
    [Range(1, 4)]
    [SerializeField]
    private int numberOfLocalPlayers = 1;

    /// <summary>
    /// Default connection state
    /// </summary>
    private static ConnectionState state = ConnectionState.IDLE;


    Color[] playerColors = new Color[] { Color.blue, Color.red, Color.green, Color.yellow };
    #endregion

    #region Public Methods
    /// <summary>
    /// Enters single player mode with predefined number of players in split screen
    /// </summary>
    public void SinglePlayerMode()
    {
        DisconnectFromPhoton();
    }

    /// <summary>
    /// Connects to the PhotonServer and create room with default RoomOptions
    /// </summary>
    public void CreateGame()
    {
        // Switch the connection state to CREATE, so when callback occur, can easily distinguish which action to take
        state = ConnectionState.CREATE;
        if (!PhotonNetwork.connected)
        {
            ConnectingToPhotonServer();
        }
    }

    /// <summary>
    /// Connects to the PhotonServer and join exisiting random room
    /// </summary>
    public void JoinRoom()
    {
        state = ConnectionState.JOIN;
        if (!PhotonNetwork.connected)
        {
            ConnectingToPhotonServer();
        }

    }
    #endregion

    #region Private Methods
    private void Start()
    {
        // Register callback function when scene changes
        SceneManager.sceneLoaded += this.OnLevelLoaded;
    }

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
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();

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
    /// Levelload callback function
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="sceneMode"></param>
    private void OnLevelLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        Debug.Log("Scene Loaded");
        
        if (offlineMode)
        {
            MatchManager.instance.SpawnLocalPlayers(playerPrefabName, numberOfLocalPlayers);
        }
        else if (PhotonNetwork.isMasterClient)
        {
            MatchManager.instance.SpawnPlayer(playerPrefabName);
        }
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
    #endregion

    #region Photon SDK Overrides
    /// <summary>
    /// Overrides OnJoinRoom which is called when client joins a room (including host)
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        base.OnJoinedRoom();

        // Set custom property to identify player's color
        ExitGames.Client.Photon.Hashtable playerInfo = new ExitGames.Client.Photon.Hashtable();

        // Unity Color cannot be serailized through photon, manual serializing it
        Debug.Log(PhotonNetwork.playerList.Length);
        Color c = playerColors[PhotonNetwork.playerList.Length - 1];
        float[] serializedColor = new float[4];
        serializedColor[0] = c.r;
        serializedColor[1] = c.g;
        serializedColor[2] = c.b;
        serializedColor[3] = c.a;

        playerInfo.Add("Color", serializedColor);
        PhotonNetwork.player.SetCustomProperties(playerInfo);

        // The host will call the change scene
        if (PhotonNetwork.isMasterClient)
        {
            // Load new scene
            PhotonNetwork.LoadLevel(onlineSceneName);
        }
        else
        {
            MatchManager.instance.SpawnPlayer(playerPrefabName);
        }
    }

    /// <summary>
    /// Override OnLeftRoom which is called when client left a room
    /// </summary>
    public override void OnLeftRoom()
    {
        Debug.Log("Left Room");
        base.OnLeftRoom();

        // Raise an event across current in-game players to notify a player has left
        RaiseEventOptions evtOptions = new RaiseEventOptions();
        evtOptions.Receivers = ReceiverGroup.All;
        PhotonNetwork.RaiseEvent((byte)ENetworkEventCode.OnRemovePlayerFromMatch, null, true, evtOptions);
    }

    /// <summary>
    /// Override OnCreateRoom, which is called when player is hosting a new room
    /// </summary>
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        base.OnCreatedRoom();
    }

    /// <summary>
    /// Override OnPhotonJoinRoomFailed, which is called when connection error occurred when attempting to join a room
    /// </summary>
    /// <param name="codeAndMsg"></param>
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

    /// <summary>
    /// Overrides the OnJoinedLobby event, which is called when connecting to PhotonServer for the first time
    /// Using ConnectionState to determine which action to take
    /// </summary>
    public override void OnJoinedLobby()
    {
        Debug.Log("JoinedLobby");
        base.OnJoinedLobby();

        if (state == ConnectionState.CREATE)
        {
            CreateRoomInPhotonServer();
        }
        else if(state == ConnectionState.JOIN)
        {
            JoinRandomGameInPhotonServer();
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        base.OnPhotonPlayerDisconnected(otherPlayer);

        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.All;
        PhotonNetwork.RaiseEvent((byte)ENetworkEventCode.OnRemovePlayerFromMatch, otherPlayer, true, options);
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log("Player Joined: " + newPlayer.ID);
        base.OnPhotonPlayerConnected(newPlayer);

        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.All;
        PhotonNetwork.RaiseEvent((byte)ENetworkEventCode.OnAddPlayerToMatch, newPlayer, true, options);
    }

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
    }
    #endregion
}

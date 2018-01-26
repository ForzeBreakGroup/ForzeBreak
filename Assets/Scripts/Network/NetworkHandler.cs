using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Extending from NetworkManager for handling room creation, room listings, host connections and client connections
 * Requires MatchHandler script as component
 */
 [RequireComponent(typeof(MatchMakerHandler))]
public class NetworkHandler : NetworkManager
{
    #region Public Members
    /// <summary>
    /// Self-Initializing singleton instance of NetworkHandler
    /// </summary>
    public static NetworkHandler instance
    {
        get
        {
            // Initializes the instance if it's first time loading
            if (!networkHandler)
            {
                // Find the GameOject in scene with NetworkHandler script attached
                networkHandler = FindObjectOfType(typeof(NetworkHandler)) as NetworkHandler;

                if (!networkHandler)
                {
                    // Error Handling
                    Debug.LogError("There needs to be one active NetworkHandler script on a GameObject in scene.");
                }
                else
                {
                    // Initialize the default parameters
                    networkHandler.Init();
                }
            }

            return networkHandler;
        }
    }

    public bool isTesting = false;
    #endregion

    #region Private Members
    /// <summary>
    /// Singleton instance of this script
    /// </summary>
    private static NetworkHandler networkHandler;
    private Dictionary<NetworkConnection, GameObject> playerInConnection;
    #endregion

    #region Public Methods
    public void RespawnPlayer(NetworkConnection conn, GameObject playerObject)
    {
        GameObject newPlayer = Instantiate<GameObject>(playerPrefab);
        NetworkServer.ReplacePlayerForConnection(conn, newPlayer, 0);
        Destroy(playerObject);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Initializes the internal members on first call
    /// </summary>
    private void Init()
    {
        playerInConnection = new Dictionary<NetworkConnection, GameObject>();
    }

    /// <summary>
    /// Unity Hook function that gets called when this GameObject is awaken by the Engine
    /// Using this hook to register the gameobject as don't destroy when scene changes
    /// </summary>
    private void Awake()
    {
        // Preserve the gameobject through the scene loadings
        DontDestroyOnLoad(this);
        Debug.Log(NetworkHandler.instance.isNetworkActive);
        if (isTesting && !NetworkHandler.instance.isNetworkActive)
        {
            NetworkHandler.instance.StartHost();
        }
    }

    private GameObject AddPlayerToScene(NetworkConnection conn, short playerControllerId)
    {
        GameObject player = null;

        if ((UnityEngine.Object) NetworkHandler.instance.playerPrefab == (UnityEngine.Object)null)
        {
            Debug.LogError((object)"The PlayerPrefab is empty on the NetworkManager. Please setup a PlayerPrefab object.");
        }
        else if ((UnityEngine.Object)NetworkHandler.instance.playerPrefab.GetComponent<NetworkIdentity>() == (UnityEngine.Object)null)
        {
            Debug.LogError((object)"The PlayerPrefab does not have a NetworkIdentity. Please add a NetworkIdentity to the player prefab.");
        }
        else if ((int)playerControllerId < conn.playerControllers.Count && conn.playerControllers[(int)playerControllerId].IsValid && (UnityEngine.Object)conn.playerControllers[(int)playerControllerId].gameObject != (UnityEngine.Object)null)
        {
            Debug.LogError((object)"There is already a player at that playerControllerId for this connections.");
        }
        else
        {
            // Obtain StartPosition from the NetworkManager
            Transform startPosition = this.GetStartPosition();

            // Spawns player at startposition location if exist otherwise 0,0,0
            player = !((UnityEngine.Object)startPosition != (UnityEngine.Object)null) ?
                    (GameObject)UnityEngine.Object.Instantiate((UnityEngine.Object)NetworkHandler.instance.playerPrefab, Vector3.zero, Quaternion.identity) :
                    (GameObject)UnityEngine.Object.Instantiate((UnityEngine.Object)NetworkHandler.instance.playerPrefab, startPosition.position, startPosition.rotation);

            // Calls the server to add the player
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }

        return player;
    }
    #endregion

    #region NetworkManager Override Functions
    public override void OnStartHost()
    {
        base.OnStartHost();
        Debug.Log("OnStartHost");
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("OnStartServer");
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        Debug.Log("OnServerConnect");
    }

    public override void OnStartClient(NetworkClient client)
    {
        base.OnStartClient(client);
        Debug.Log("OnStartClient");
    }

    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        base.OnMatchCreate(success, extendedInfo, matchInfo);
        Debug.Log("OnMatchCreate");
    }

    public override void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        base.OnMatchList(success, extendedInfo, matchList);
        Debug.Log("OnMatchList");
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        Debug.Log("OnClientConnect");
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject player = AddPlayerToScene(conn, playerControllerId);
        Debug.Log("OnServerAddPlayer");

        // Register the player in the network list
        foreach(KeyValuePair<NetworkConnection, GameObject> entry in playerInConnection)
        {
            player.GetComponent<ArrowIndicators>().RpcAddPlayer(entry.Key.connectionId, entry.Value);
            entry.Value.GetComponent<ArrowIndicators>().RpcAddPlayer(conn.connectionId, player);
        }

        // Update the internal record
        NetworkHandler.instance.playerInConnection.Add(conn, player);

    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log("OnServerDisconnect");

        // Remove the player connection from the record
        NetworkHandler.instance.playerInConnection.Remove(conn);

        // Calls to all players still in network to update the arrow indicator to lose track of left player
        foreach(KeyValuePair<NetworkConnection, GameObject> entry in playerInConnection)
        {
            entry.Value.GetComponent<ArrowIndicators>().RpcRemovePlayer(conn.connectionId);
        }

    }
    #endregion
}

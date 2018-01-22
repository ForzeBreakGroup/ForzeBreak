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

    /// <summary>
    /// Singleton instance of MatchMakerHandler
    /// </summary>
    private static MatchMakerHandler matchMakerHandler;
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
        matchMakerHandler = GetComponent<MatchMakerHandler>();
    }

    /// <summary>
    /// Unity Hook function that gets called when this GameObject is awaken by the Engine
    /// Using this hook to register the gameobject as don't destroy when scene changes
    /// </summary>
    private void Awake()
    {
        // Preserve the gameobject through the scene loadings
        DontDestroyOnLoad(this);

        if (isTesting)
        {
            NetworkHandler.instance.StartHost();
        }
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
        base.OnServerAddPlayer(conn, playerControllerId);
    }
    #endregion
}

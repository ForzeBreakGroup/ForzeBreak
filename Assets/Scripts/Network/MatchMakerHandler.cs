using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

/*
 * Author: Jason Lin
 * 
 * Description：
 * Class to handle matchmaker calls and callback functions
 */
public class MatchMakerHandler : MonoBehaviour
{
    #region Public Members
    // MatchMaking room name
    public static string RoomName = "Default";

    // Singleton instance of MatchmakerHandler
    public static MatchMakerHandler instance
    {
        get
        {
            if (!matchHandler)
            {
                matchHandler = FindObjectOfType(typeof(MatchMakerHandler)) as MatchMakerHandler;
            }

            if (!matchHandler)
            {
                Debug.LogError("MatchHandler Script Must Be Attached To A GameObject In Scene.");
            }

            return matchHandler;
        }
    }
    #endregion

    #region Private Members
    // Private instance reference
    private static MatchMakerHandler matchHandler;

    // List of matches available through Unity Multiplayer service
    private List<MatchInfoSnapshot> matchList = new List<MatchInfoSnapshot>();

    // Reference to NetworkHandler in scene
    private NetworkHandler networkHandler;

    // Internally stored MatchInfo from creating/joining match
    private MatchInfo matchInfo;
    #endregion

    #region Public Methods
    /// <summary>
    /// Public method to create a match using RoomName provided
    /// </summary>
    public void HostGame()
    {
        Debug.Log(networkHandler.matchMaker);
        networkHandler.matchMaker.CreateMatch(RoomName, 4, true, "", "", "", 0, 0, this.OnMatchCreated);
    }

    /// <summary>
    /// Public method to obtain a list of available matches
    /// </summary>
    public void ListGames()
    {
        networkHandler.matchMaker.ListMatches(0, 20, "", true, 0, 0, this.OnMatchList);
    }

    /// <summary>
    /// Joining a match from the list
    /// </summary>
    public void JoinGame()
    {
        if (matchList.Count > 0)
        {
            networkHandler.matchMaker.JoinMatch(matchList[0].networkId, "", "", "", 0, 0, this.OnMatchJoined);
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Auto start a matchmaker
    /// </summary>
    private void Awake()
    {
        if (!networkHandler)
        {
            networkHandler = FindObjectOfType(typeof(NetworkHandler)) as NetworkHandler;
        }
        networkHandler.StartMatchMaker();
    }

    /// <summary>
    /// MatchMaker OnCreate callback function
    /// </summary>
    /// <param name="success"> Boolean for if MatchCreated </param>
    /// <param name="extendedInfo"> String detailing explicit error message if any </param>
    /// <param name="matchInfo"> MatchInfo from the match created </param>
    private void OnMatchCreated(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        Debug.Log("MatchMakerHandler OnMatchCreated " + matchInfo);

        // Calls the NetworkHandler for starting the host service on successful connection
        if (success)
        {
            Utility.SetAccessTokenForNetwork(matchInfo.networkId, matchInfo.accessToken);
            networkHandler.StartHost(matchInfo);
        }
        else
        {
            Debug.LogError("MatchMakerHandler OnMatchCreated Failed: " + extendedInfo);
        }
    }

    private void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        Debug.Log("MatchMakerHandler OnMatchList");
        if (success)
        {
            this.matchList = matches;
            foreach(MatchInfoSnapshot match in this.matchList)
            {
                Debug.Log(match.name);
            }
        }
    }

    private void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        Debug.Log("MatchMakerHandler OnMatchJoined " + matchInfo);

        // Calls the NetworkHandler for joining an existing match on successful connection
        if (success)
        {
            Utility.SetAccessTokenForNetwork(matchInfo.networkId, matchInfo.accessToken);
            networkHandler.StartClient(matchInfo);
        }
        else
        {
            Debug.LogError("MatchMakerHandler OnMatchJoined Faield: " + extendedInfo);
        }
    }

    private void OnDestroy()
    {
        
    }
    #endregion
}

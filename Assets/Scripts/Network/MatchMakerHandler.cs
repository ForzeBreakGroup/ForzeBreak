using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class MatchMakerHandler : MonoBehaviour
{
    #region Public Members
    public static string RoomName = "Default";
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
    private static MatchMakerHandler matchHandler;

    private List<MatchInfoSnapshot> matchList = new List<MatchInfoSnapshot>();
    private NetworkHandler networkHandler;
    private MatchInfo matchInfo;
    #endregion

    #region Public Methods
    public void HostGame()
    {
        Debug.Log(networkHandler.matchMaker);
        networkHandler.matchMaker.CreateMatch(RoomName, 4, true, "", "", "", 0, 0, networkHandler.OnMatchCreate);
    }

    public void ListGames()
    {

    }

    public void JoinGame()
    {

    }
    #endregion

    #region Private Methods
    private void Awake()
    {
        if (!networkHandler)
        {
            networkHandler = FindObjectOfType(typeof(NetworkHandler)) as NetworkHandler;
        }
        networkHandler.StartMatchMaker();
    }

    private void OnDestroy()
    {
        
    }
    #endregion
}

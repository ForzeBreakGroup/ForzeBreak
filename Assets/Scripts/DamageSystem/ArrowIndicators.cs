﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

/*
 * Author: Jason Lin
 * 
 * Description:
 * 3D arrow indicator manager to handle multiple arrow pointing at the given player's position
 * Methods to add/remove arrow
 */

public class ArrowIndicators : Photon.MonoBehaviour
{
    #region Private Members

    /// <summary>
    /// Dictioanry to keep track of which connection id is connecting to which arrow
    /// </summary>
    private Dictionary<int, GameObject> arrowList;

    #endregion

    #region Public Members

    /// <summary>
    /// Arrow Indicator GameObject
    /// </summary>
    public GameObject arrowIndicator;

    #endregion

    #region Private Methods
    /// <summary>
    /// Initializes ArrowList dictionary
    /// </summary>
    private void Awake()
    {
        if (!arrowIndicator)
        {
            Debug.LogError("Arrow Indicator Gameobject Prefab Cannot be Found");
        }
        arrowList = new Dictionary<int, GameObject>();
    }

    /// <summary>
    /// Start listening to photon events
    /// </summary>
    private void OnEnable()
    {
        PhotonNetwork.OnEventCall += EvtAddPlayerToMatchHandler;
        PhotonNetwork.OnEventCall += EvtRemovePlayerFromMatchHandler;
    }

    /// <summary>
    /// Stops listening to photon events
    /// </summary>
    private void OnDisable()
    {
        PhotonNetwork.OnEventCall -= EvtAddPlayerToMatchHandler;
        PhotonNetwork.OnEventCall -= EvtRemovePlayerFromMatchHandler;
    }

    /// <summary>
    /// Photon Event handler for OnPlayerSpawnFinished
    /// Loop through the scene to find all player objects and start tracking the new player object
    /// </summary>
    /// <param name="evtCode"></param>
    /// <param name="content"></param>
    /// <param name="senderid"></param>
    private void EvtAddPlayerToMatchHandler(byte evtCode, object content, int senderid)
    {
        if (evtCode == (byte)ENetworkEventCode.OnPlayerSpawnFinished && photonView.isMine)
        {
            // Loop through all objects in game to make sure all players are included
            NetworkPlayerData[] playersInGame = FindObjectsOfType<NetworkPlayerData>();
            foreach (NetworkPlayerData p in playersInGame)
            {
                // If the photonView is not myself, means it's other player
                if (!p.photonView.isMine)
                {
                    // If the dictionary record does not have the photonView ID start tracking it
                    if (!arrowList.ContainsKey(p.photonView.ownerId))
                    {
                        // Create an arrow for this specific remote client
                        GameObject arrow = Instantiate(arrowIndicator, this.transform);
                        arrow.transform.localScale = new Vector3(1, 1, 1);
                        arrow.GetComponent<TrackPlayer>().AssignTarget(p.gameObject);
                        arrowList.Add(p.photonView.ownerId, arrow);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Photon Event handler for OnRemovePLayerFromMatch
    /// Use the player left the match to remove the arrow corresponding to it
    /// </summary>
    /// <param name="evtCode"></param>
    /// <param name="content"></param>
    /// <param name="senderid"></param>
    private void EvtRemovePlayerFromMatchHandler(byte evtCode, object content, int senderid)
    {
        if (evtCode == (byte)ENetworkEventCode.OnRemovePlayerFromMatch && photonView.isMine)
        {
            PhotonPlayer otherPlayer = (PhotonPlayer)content;

            // Find the arrow matching the player left the game then destroy it
            if (arrowList.ContainsKey(otherPlayer.ID))
            {
                Destroy(arrowList[otherPlayer.ID]);
            }
        }
    }

    #endregion
}

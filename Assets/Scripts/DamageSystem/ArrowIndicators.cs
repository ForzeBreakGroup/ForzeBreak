using System.Collections;
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
    /// <summary>
    /// Arrow Indicator GameObject
    /// </summary>
    public GameObject arrowIndicator;

    /// <summary>
    /// Dictioanry to keep track of which connection id is connecting to which arrow
    /// </summary>
    private Dictionary<int, GameObject> arrowList;

    private void Awake()
    {
        if (!arrowIndicator)
        {
            Debug.LogError("Arrow Indicator Gameobject Prefab Cannot be Found");
        }
        arrowList = new Dictionary<int, GameObject>();
    }

    private void OnEnable()
    {
        PhotonNetwork.OnEventCall += EvtAddPlayerToMatchHandler;
        PhotonNetwork.OnEventCall += EvtRemovePlayerFromMatchHandler;
    }

    private void OnDisable()
    {
        PhotonNetwork.OnEventCall -= EvtAddPlayerToMatchHandler;
        PhotonNetwork.OnEventCall -= EvtRemovePlayerFromMatchHandler;
    }

    private void EvtAddPlayerToMatchHandler(byte evtCode, object content, int senderid)
    {
        if (evtCode == (byte)ENetworkEventCode.OnPlayerSpawnFinished && photonView.isMine)
        {
            // Loop through all objects in game to make sure all players are included
            NetworkPlayerData[] playersInGame = FindObjectsOfType<NetworkPlayerData>();
            foreach(NetworkPlayerData p in playersInGame)
            {
                if (!p.photonView.isMine)
                {
                    if (!arrowList.ContainsKey(p.photonView.ownerId))
                    {
                        GameObject arrow = Instantiate(arrowIndicator, this.transform);
                        arrow.GetComponent<TrackPlayer>().AssignTarget(p.gameObject);
                        arrowList.Add(p.photonView.ownerId, arrow);
                    }
                }
            }
        }
    }

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
}

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

public class ArrowIndicationSystem : Photon.MonoBehaviour
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
        PhotonNetwork.OnEventCall += EvtRemovePlayerFromMatchHandler;
    }

    /// <summary>
    /// Stops listening to photon events
    /// </summary>
    private void OnDisable()
    {
        PhotonNetwork.OnEventCall -= EvtRemovePlayerFromMatchHandler;
    }

    public void UpdateArrowList()
    {
        int playerNum = GetComponent<CarUserControl>().controllerNum;

        // Loop through all objects in game to make sure all players are included
        NetworkPlayerData[] playersInGame = FindObjectsOfType<NetworkPlayerData>();
        foreach (NetworkPlayerData p in playersInGame)
        {
            // If the photonView is not myself, means it's other player
            if (!NetworkManager.instance.ValidateOwnership(p.photonView, playerNum))
            {
                // If the dictionary record does not have the photonView ID start tracking it
                if (!arrowList.ContainsKey(p.photonView.ownerId))
                {
                    GameObject arrow = SpawnArrowToTrackPlayer(p.gameObject);
                    arrowList.Add(p.photonView.ownerId, arrow);
                }

                // If the dictionary record contains the ID and the arrow does not exist, means the player just respawned
                else if (arrowList[p.photonView.ownerId] == null)
                {
                    GameObject arrow = SpawnArrowToTrackPlayer(p.gameObject);
                    arrowList[p.photonView.ownerId] = arrow;
                }
            }
        }
    }

    private GameObject SpawnArrowToTrackPlayer(GameObject target)
    {
        // Create an arrow for this specific remote client
        GameObject arrow = Instantiate(arrowIndicator, transform.position, Quaternion.identity);
        arrow.transform.SetParent(transform);
        //arrow.transform.localScale = new Vector3(1, 1, 1);
        arrow.GetComponent<TrackPlayer>().AssignTarget(target);

        return arrow;
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
            Debug.Log("Arrow Indicator Remove Player");
            PhotonPlayer otherPlayer = (PhotonPlayer)content;

            // Find the arrow matching the player left the game then destroy it
            if (arrowList.ContainsKey(otherPlayer.ID))
            {
                Debug.Log("Remove Arrow Indicator");
                Destroy(arrowList[otherPlayer.ID]);
                arrowList.Remove(otherPlayer.ID);
            }
        }
    }

    #endregion
}

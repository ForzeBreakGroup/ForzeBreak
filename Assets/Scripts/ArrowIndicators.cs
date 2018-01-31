using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/*
 * Author: Jason Lin
 * 
 * Description:
 * 3D arrow indicator manager to handle multiple arrow pointing at the given player's position
 * Methods to add/remove arrow
 */

public class ArrowIndicators : NetworkBehaviour
{
    /// <summary>
    /// Arrow Indicator GameObject
    /// </summary>
    public GameObject arrowIndicator;

    /// <summary>
    /// Dictioanry to keep track of which connection id is connecting with which player object
    /// </summary>
    private Dictionary<int, GameObject> arrowList;

    /// <summary>
    /// Array of tracker scheme to be placed based on number of player
    /// </summary>
    private TrackerScheme[] trackerScheme = { TrackerScheme.Blue, TrackerScheme.Red, TrackerScheme.Green };

    private void Awake()
    {
        if (!arrowIndicator)
        {
            Debug.LogError("Arrow Indicator Gameobject Prefab Cannot be Found");
        }
        arrowList = new Dictionary<int, GameObject>();
    }

    [ClientRpc]
    public void RpcAddPlayer(int uniqueId, GameObject playerObject)
    {
        if (!isLocalPlayer)
            return;

        GameObject trackPlayer = Instantiate(arrowIndicator, this.transform);
        trackPlayer.GetComponent<TrackPlayer>().objectToTrack = playerObject;
        trackPlayer.GetComponent<TrackPlayer>().ChangeScheme(trackerScheme[arrowList.Count + 1]);
        arrowList.Add(uniqueId, trackPlayer);
    }

    [ClientRpc]
    public void RpcRemovePlayer(int uniqueId)
    {
        if (!isLocalPlayer)
            return;

        GameObject arrow = null;
        if (arrowList.TryGetValue(uniqueId, out arrow))
        {
            Destroy(arrow);
        }

        arrowList.Remove(uniqueId);
    }
}

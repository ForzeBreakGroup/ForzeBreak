using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ArrowIndicators : NetworkBehaviour
{
    public GameObject arrowIndicator;
    // Dictionary of <player unique Network ID, Arrow Object>
    Dictionary<int, GameObject> arrowList;

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Jason Lin
 * 
 * Description:
 * 3D arrow indicator manager to handle multiple arrow pointing at the given player's position
 * Methods to add/remove arrow
 */

public class ArrowIndicators : MonoBehaviour
{
    /// <summary>
    /// Arrow Indicator GameObject
    /// </summary>
    public GameObject arrowIndicator;

    /// <summary>
    /// Dictioanry to keep track of which connection id is connecting with which player object
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
    
    public void RpcAddPlayer(int uniqueId, GameObject playerObject)
    {
        GameObject trackPlayer = Instantiate(arrowIndicator, this.transform);
        CarUserControl carControl = playerObject.GetComponent<CarUserControl>();
        trackPlayer.GetComponent<TrackPlayer>().objectToTrack = playerObject;
        trackPlayer.GetComponent<TrackPlayer>().ChangeScheme(carControl.color);
        carControl.ChangeColor(carControl.color);
        arrowList.Add(uniqueId, trackPlayer);
    }

    public void RpcRemovePlayer(int uniqueId)
    {
        GameObject arrow = null;
        if (arrowList.TryGetValue(uniqueId, out arrow))
        {
            Destroy(arrow);
        }

        arrowList.Remove(uniqueId);
    }
}

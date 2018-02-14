using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Tracking GameObject provided by the caller, points towards the object
 */
public class TrackPlayer : MonoBehaviour
{
    /// <summary>
    /// Game Object to track with
    /// </summary>
    public GameObject objectToTrack;

    /// <summary>
    /// Changes the arrow to predetermined scheme option
    /// </summary>
    /// <param name="scheme"></param>
    public void ChangeScheme(Color c)
    {
        Material mat = transform.Find("Arrow_Test").GetComponent<MeshRenderer>().material;

        if (mat)
        {
            mat.color = c;
        }
    }

    private void Update()
    {
        // Start tracking if target is provided
        if (objectToTrack)
        {
            transform.LookAt(objectToTrack.transform);
        }
    }
}

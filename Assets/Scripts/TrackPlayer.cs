using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Tracking GameObject provided by the caller, points towards the object
 */

/// <summary>
/// Enum defines different tracker scheme options available
/// </summary>
public enum TrackerScheme
{
    Red,
    Blue,
    Green
};

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
    public void ChangeScheme(TrackerScheme scheme)
    {
        Material mat = transform.Find("Arrow_Test").GetComponent<MeshRenderer>().material;

        if (mat)
        {
            // Changes the arrow texture
            switch (scheme)
            {
                case TrackerScheme.Blue:
                    mat.color = Color.blue;
                    break;
                case TrackerScheme.Red:
                    mat.color = Color.red;
                    break;
                case TrackerScheme.Green:
                    mat.color = Color.green;
                    break;
                default:
                    break;
            }
        }
    }

    private void Update()
    {
        // Start tracking if target is provided
        if (objectToTrack)
        {
            transform.LookAt(objectToTrack.transform);
            objectToTrack.GetComponent<Renderer>();
        }
    }
}

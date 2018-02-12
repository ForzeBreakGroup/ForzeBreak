using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Jason Lin
 * 
 * Description:
 * For setting spawnpoint in map for MatchManager to lookup for
 */
public class NetworkSpawnPoint : MonoBehaviour
{
    /// <summary>
    /// Public position for easier accessibility
    /// </summary>
    public Vector3 spawnPoint;

    /// <summary>
    /// Public rotation for easier accessibility
    /// </summary>
    public Quaternion spawnRotation;

    // Use this for initialization
    void Awake ()
    {
        // Make the gameobject facing the center of arena
        transform.LookAt(Vector3.zero);

        // Obtain the position and rotation after looking at the center of arena
        spawnPoint = transform.localPosition;
        spawnRotation = transform.localRotation;
    }
}

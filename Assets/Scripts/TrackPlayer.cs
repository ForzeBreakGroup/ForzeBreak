using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPlayer : MonoBehaviour
{
    public GameObject objectToTrack;

    private void Update()
    {
        if (objectToTrack)
        {
            transform.LookAt(objectToTrack.transform);
            objectToTrack.GetComponent<Renderer>();
        }
    }
}

﻿using System.Collections;
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
    [SerializeField] private GameObject objectToTrack;

    [SerializeField] private float minScale = 0.3f;
    [SerializeField] private float maxScale = 1.0f;

    private bool trackingInitialized = false;
    private GameObject arrow;

    private void Awake()
    {
        arrow = transform.Find("Arrow").gameObject;
    }

    /// <summary>
    /// Assign the target to the arrow to follow, the arrow changes it's color accordingly
    /// </summary>
    /// <param name="target"></param>
    public void AssignTarget(GameObject target)
    {
        if (!target)
        {
            Debug.LogError("Target Does not Exist");
        }

        // Deserialize color property obtained from target player
        Color c;
        if (NetworkManager.offlineMode)
        {
            c = NetworkManager.instance.GetPlayerColor(target.GetComponent<CarUserControl>().controllerNum - 1);
        }
        else
        {
            float[] serializedColor = PhotonPlayer.Find(target.GetPhotonView().ownerId).CustomProperties["Color"] as float[];
            c = new Color(serializedColor[0], serializedColor[1], serializedColor[2], serializedColor[3]);
        }
        transform.Find("Arrow").GetComponent<Renderer>().material.color = c;

        objectToTrack = target;
        GetComponentInChildren<ReticleSystem>().EnableReticleSystem(objectToTrack);

        trackingInitialized = true;
    }

    private void FixedUpdate()
    {
        // Start tracking if target is provided
        if (objectToTrack)
        {
            transform.LookAt(objectToTrack.transform);

            // Clamp the scale
            float scale = 10.0f / Vector3.Distance(this.transform.position, objectToTrack.transform.position);
            scale = Mathf.Clamp(scale, minScale, maxScale);

            arrow.transform.localScale = new Vector3(scale, scale, scale);
        }

        if (trackingInitialized && !objectToTrack)
        {
            Destroy(this.gameObject);
        }
    }
}

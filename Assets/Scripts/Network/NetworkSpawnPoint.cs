using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSpawnPoint : MonoBehaviour
{
    public Vector3 spawnPoint;
    public Quaternion spawnRotation;

    // Use this for initialization
    void Awake ()
    {
        transform.LookAt(Vector3.zero);

        spawnPoint = transform.localPosition;
        spawnRotation = transform.localRotation;
    }
}

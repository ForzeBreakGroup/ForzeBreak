using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeRamCollision : MonoBehaviour
{
    public delegate void SpikeRamCollisionCallback(Collision collision);
    public SpikeRamCollisionCallback callbackFunc;

    private void OnCollisionEnter(Collision collision)
    {
        callbackFunc(collision);
    }
}

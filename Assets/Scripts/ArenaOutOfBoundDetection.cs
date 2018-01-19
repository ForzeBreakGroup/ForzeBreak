using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaOutOfBoundDetection : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerLife pl = other.GetComponentInParent<PlayerLife>();
            pl.DecrementPlayerLife();
        }
    }
}

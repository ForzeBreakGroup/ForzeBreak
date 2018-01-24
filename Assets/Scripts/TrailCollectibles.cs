using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailCollectibles : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Destroy(this.gameObject);
        }
    }
}

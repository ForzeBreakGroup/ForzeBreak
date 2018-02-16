using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class ImpactExplosion : Photon.MonoBehaviour
{
    [SerializeField] private float destroyDelay = 0.5f;
    private void Awake()
    {
        StartCoroutine(DestroyAfterSeconds());
    }

    private void Update()
    {
    }

    IEnumerator DestroyAfterSeconds()
    {
        yield return new WaitForSeconds(destroyDelay);
        if (photonView.isMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}

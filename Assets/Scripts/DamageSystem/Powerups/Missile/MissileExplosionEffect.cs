using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class MissileExplosionEffect : Photon.MonoBehaviour
{
    [SerializeField] private float ExplosionForce = 2500;
    [SerializeField] private float ExplosionRadius = 30;

    private void OnTriggerEnter(Collider other)
    {
        //PhotonView otherPhotonView = other.transform.root.gameObject.GetPhotonView();
        if (other.transform.root.tag == "Player")
        {
            Vector3 impactCenter = transform.position;
            other.transform.root.GetComponent<DamageSystem>().CreateExplosion(ExplosionForce, transform.position, ExplosionRadius);
            //otherPhotonView.RPC("CreateExplosion", PhotonPlayer.Find(otherPhotonView.viewID), ExplosionForce, transform.position, ExplosionRadius);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class MissileExplosionEffect : Photon.MonoBehaviour
{
    [Range(10, 100)]
    [SerializeField] private float ExplosionForce = 25;

    [Range(0.1f, 10.0f)]
    [SerializeField] private float ExplosionRadius = 3;
    [SerializeField] private float ExplosionDuration = 0.2f;

    private bool stillInEffect = false;
    private float elapsedTime = 0f;

    private void OnTriggerEnter(Collider other)
    {
        PhotonView otherPhotonView = other.transform.root.gameObject.GetPhotonView();
        if (other.transform.root.tag == "Player" && !stillInEffect && otherPhotonView.isMine)
        {
            Vector3 impactCenter = transform.position;
            other.transform.root.GetComponent<DamageSystem>().ApplyDamageForce(ExplosionForce, transform.position, ExplosionRadius, 0, "Missile");
        }
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= ExplosionDuration)
        {
            stillInEffect = true;
        }
    }
}

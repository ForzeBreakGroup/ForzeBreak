﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class MissileExplosionEffect : Photon.MonoBehaviour
{
    [SerializeField] private float ExplosionForce = 2500;
    [SerializeField] private float ExplosionRadius = 30;
    [SerializeField] private float ExplosionDuration = 0.2f;

    private bool stillInEffect = false;
    private float elapsedTime = 0f;

    private void OnTriggerEnter(Collider other)
    {
        PhotonView otherPhotonView = other.transform.root.gameObject.GetPhotonView();
        if (other.transform.root.tag == "Player" && !stillInEffect && otherPhotonView.isMine)
        {
            Vector3 impactCenter = transform.position;
            other.transform.root.GetComponent<DamageSystem>().CreateExplosion(ExplosionForce, transform.position, ExplosionRadius);
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

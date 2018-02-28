﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeRamComponent : PowerUpBase
{
    [Range(0, 50000)]
    [SerializeField] private int damage = 10000;

    [Range(1, 4)]
    [SerializeField]
    private int capacity = 2;

    public override void AdjustModel()
    {
        transform.localPosition = componentOffset;
        transform.localRotation = Quaternion.identity;
    }

    private void OnTriggerEnter(Collider other)
    {
        PhotonView otherPhotonView = other.transform.root.gameObject.GetPhotonView();
        if (other.transform.root.tag == "Player")
        {
            Debug.Log("Spike RAM!");
            otherPhotonView.RPC("CreateExplosion", PhotonPlayer.Find(otherPhotonView.viewID), (float)damage, transform.position, 30.0f);
            --capacity;
        }
    }
}

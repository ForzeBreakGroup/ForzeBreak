using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class PowerUpCollectible : Photon.MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Sending RPC");
            photonView.RPC("AddPowerUpComponent", PhotonPlayer.Find(other.transform.root.gameObject.GetPhotonView().viewID));
        }
    }
}

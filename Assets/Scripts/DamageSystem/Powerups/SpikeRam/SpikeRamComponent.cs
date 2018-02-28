using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeRamComponent : PowerUpBase
{
    [Range(0, 50000)]
    [SerializeField] private int damage = 10000;

    public override void AdjustModel()
    {
        transform.localPosition = componentOffset;
        transform.localRotation = Quaternion.identity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.root.tag == "Player")
        {
            PhotonView otherPhotonView = collision.transform.root.gameObject.GetPhotonView();
            otherPhotonView.RPC("CreateExplosion", PhotonPlayer.Find(otherPhotonView.viewID), damage, collision.contacts[0].point, 3.0f);
        }
    }
}

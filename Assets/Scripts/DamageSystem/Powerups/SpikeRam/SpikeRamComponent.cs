using System.Collections;
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

        // Attach ram spike collision script to the parent object
        transform.root.gameObject.AddComponent<SpikeRamCollision>().callbackFunc = OnSpikeRamCollision;
    }

    private void OnSpikeRamCollision(Collision collision)
    {
        PhotonView otherPhotonView = collision.transform.root.gameObject.GetPhotonView();
        if (collision.transform.root.tag == "Player" && otherPhotonView.isMine)
        {
            otherPhotonView.RPC("CreateExplosion", PhotonPlayer.Find(otherPhotonView.viewID), (float)damage, transform.position, 30.0f);
            --capacity;

            if (capacity <= 0)
            {
                UnloadPowerUp();
            }
        }
    }

    protected override void UnloadPowerUp()
    {
        base.UnloadPowerUp();
        Destroy(transform.root.gameObject.GetComponent<SpikeRamCollision>());
    }
}

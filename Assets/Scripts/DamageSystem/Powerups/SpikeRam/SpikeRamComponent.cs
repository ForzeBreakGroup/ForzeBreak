using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeRamComponent : PowerUpBase
{
    [Range(10, 100)]
    [SerializeField]
    private float force = 100;

    [Range(0.1f, 10.0f)]
    [SerializeField]
    private float radius = 10.0f;

    [Range(1, 4)]
    [SerializeField]
    private int capacity = 2;

    public override void AdjustModel()
    {
        base.AdjustModel();
        //transform.localPosition = componentOffset;
        //transform.localRotation = Quaternion.identity;
    }

    private void OnTriggerEnter(Collider other)
    {
        PhotonView otherPhotonView = other.transform.root.gameObject.GetPhotonView();
        if (other.transform.root.tag == "Player" && otherPhotonView.isMine)
        {
            other.transform.root.GetComponent<DamageSystem>().ApplyDamageForce((float)force, transform.position, radius);
            --capacity;

            if (capacity <= 0)
            {
                UnloadPowerUp();
            }
        }
    }
}

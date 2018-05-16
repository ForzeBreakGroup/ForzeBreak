using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotPotatoCollision : PowerUpCollision
{

    // public GameObject explosionVFX;
    // Overriding ComponentCollision due to HotPotato will never be in contact points
    public override void ComponentCollision(Collision collision)
    {
        ((HotPotatoMovement)PowerUpMovement).TransferHotPotato(externalCollider.GetComponent<PhotonView>().ownerId);
    }

    public void Detonate()
    {
        PhotonView.RPC("RpcDetonatePotato", PhotonTargets.All);
        PlayVFX();
    }

    [PunRPC]
    public void RpcDetonatePotato()
    {
        PhotonView rootPhotonView = transform.root.gameObject.GetPhotonView();

        if (rootPhotonView.isMine)
        {
            // Moving the hot potato under the vehicle center, and apply damage
            this.transform.position = transform.parent.position;
            externalCollider = transform.parent.gameObject;
            ApplyDamage("HotPotato");

            // Destroy self
            PowerUpMovement.DestroyPowerUpProjectile();
        }

        //Instantiate(explosionVFX, this.transform.position, Quaternion.identity);
    }
}

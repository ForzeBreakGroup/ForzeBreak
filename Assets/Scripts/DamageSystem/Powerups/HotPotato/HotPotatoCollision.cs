using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotPotatoCollision : PowerUpCollision
{
    // Overriding ComponentCollision due to HotPotato will never be in contact points
    public override void ComponentCollision(Collision collision)
    {
        ((HotPotatoMovement)PowerUpMovement).TransferHotPotato(otherCollider.GetComponent<PhotonView>().ownerId);
    }

    public void Detonate()
    {
        PhotonView.RPC("RpcDetonatePotato", PhotonTargets.AllViaServer);
    }

    [PunRPC]
    public void RpcDetonatePotato()
    {
        if (PhotonView.isMine)
        {
            // Moving the hot potato under the vehicle center, and apply damage
            this.transform.position = transform.parent.position;
            otherCollider = transform.parent.gameObject;
            otherDmgSystem = otherCollider.GetComponent<DamageSystem>();

            ApplyDamage();

            // Destroy self
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}

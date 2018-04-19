using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotPotatoCollision : PowerUpCollision
{
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
            Debug.Log("Detonating");

            // Moving the hot potato under the vehicle center, and apply damage
            this.transform.position = otherCollider.transform.position;

            ApplyDamage();

            // Destroy self
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}

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
        PhotonView.RPC("RpcDetonatePotato", PhotonTargets.All);
    }

    [PunRPC]
    public void RpcDetonatePotato()
    {
        Debug.Log("Detonate");
    }
}

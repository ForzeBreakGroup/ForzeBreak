using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeRamComponent : PowerUpComponent
{
    public override void SetComponentParent(int parentID)
    {
        base.SetComponentParent(parentID);
        photonView.RPC("RpcSetCollisionOwner", PhotonTargets.All, parentID);
    }

    [PunRPC]
    public void RpcSetCollisionOwner(int ownerId)
    {
        GetComponent<SpikeRamData>().OwnerID = ownerId;
    }
}

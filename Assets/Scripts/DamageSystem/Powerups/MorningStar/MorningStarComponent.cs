using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorningStarComponent : PowerUpComponent
{

    public override void SetComponentParent(int parentID)
    {
        base.SetComponentParent(parentID);
        //photonView.RPC("RpcSetJoint", PhotonTargets.AllViaServer);
        photonView.RPC("RpcSetMornigStarOwner", PhotonTargets.All, parentID);

    }
    //[PunRPC]
    //public void RpcSetJoint()
    //{
    //    Transform t = transform.Find("joint (10)");
    //    ConfigurableJoint joint = t.GetComponent<ConfigurableJoint>();
    //    joint.connectedBody = transform.root.GetComponent<Rigidbody>();
    //}
    [PunRPC]
    public void RpcSetMornigStarOwner(int ownerId)
    {
        transform.Find("morningstar").GetComponent<MorningStarData>().OwnerID = ownerId;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpData : PowerUpProjectileBase
{
    [SerializeField]
    protected int ownerId;
    public int OwnerID
    {
        get
        {
            return ownerId;
        }

        set
        {
            ownerId = value;
        }
    }

    public void SetOwnerId(int id)
    {
        PhotonView.RPC("RpcSetOwnerId", PhotonTargets.All, id);
    }


    [PunRPC]
    public void RpcSetOwnerId(int id)
    {
        ownerId = id;
    }
}

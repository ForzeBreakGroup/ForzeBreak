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
            ownerId=value;
        }
    }


    [PunRPC]
    public void SetOwnerId(int id)
    {
        ownerId = id;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForzeBreakControl : PowerUpComponent
{
    public override void SetComponentParent(int parentID)
    {
        base.SetComponentParent(parentID);

        GameObject spawnedItem = PhotonNetwork.Instantiate(spawnItem.name, new Vector3(0, 1.5f, 0), Quaternion.identity, 0);
        ((PowerUpData)spawnedItem.GetComponent(typeof(PowerUpData))).SetOwnerId(this.ownerID);

        DecreaseCapacity();
    }
}

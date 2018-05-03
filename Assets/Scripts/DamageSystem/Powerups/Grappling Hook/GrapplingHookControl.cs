using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHookControl : PowerUpComponent
{
    protected override void OnPress()
    {
        if (spawnItem != null)
        {
            GameObject hook = PhotonNetwork.Instantiate(spawnItem.name, transform.position, transform.rotation, 0);
            ((PowerUpData)hook.GetComponent(typeof(PowerUpData))).SetOwnerId(this.ownerID);
            DecreaseCapacity();
        }
    }
}

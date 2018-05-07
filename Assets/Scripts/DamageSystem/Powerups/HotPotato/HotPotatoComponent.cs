using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotPotatoComponent : PowerUpComponent
{
    public GameObject hotPotato { get; private set; }

    public override void SetComponentParent(int parentID)
    {
        base.SetComponentParent(parentID);

        // Spawn the hot potato
        SpawnHotPotato();

        // Transfer the ownership to the owner
        hotPotato.GetComponent<HotPotatoMovement>().TransferHotPotato(GetComponent<PhotonView>().ownerId);

        // Destroy the component
        UnloadPowerUp();
    }

    public GameObject SpawnHotPotato()
    {
        // Check if hot potato has been spawned
        // If not, spawn a hot potato
        if (hotPotato == null)
        {
            hotPotato = PhotonNetwork.Instantiate(spawnItem.name, transform.position, Quaternion.identity, 0);
            ((PowerUpData)hotPotato.GetComponent(typeof(PowerUpData))).SetOwnerId(this.ownerID);
        }

        return hotPotato;
    }
}

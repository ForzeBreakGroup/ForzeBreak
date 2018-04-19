using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotPotatoComponent : PowerUpComponent
{
    public GameObject hotPotato { get; private set; }

    protected override void OnPress()
    {
        if (hotPotato == null)
        {
            SpawnHotPotato();
            hotPotato.GetComponent<HotPotatoMovement>().TransferHotPotato(GetComponent<PhotonView>().ownerId);
        }

        hotPotato.GetComponent<HotPotatoCollision>().Detonate();
    }

    public GameObject SpawnHotPotato()
    {
        // Check if hot potato has been spawned
        // If not, spawn a hot potato
        if (hotPotato == null)
        {
            hotPotato = PhotonNetwork.Instantiate(spawnItem.name, transform.position, Quaternion.identity, 0);
            ((PowerUpData)hotPotato.GetComponent(typeof(PowerUpData))).OwnerID = this.ownerID;
        }

        return hotPotato;
    }
}

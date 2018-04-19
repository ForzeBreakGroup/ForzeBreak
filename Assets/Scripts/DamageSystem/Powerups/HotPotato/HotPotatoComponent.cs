using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotPotatoComponent : PowerUpComponent
{
    public GameObject hotPotato { get; private set; }
    private bool hasDetonated = false;

    public override void SetComponentParent(int parentID)
    {
        base.SetComponentParent(parentID);

        SpawnHotPotato();
        hotPotato.GetComponent<HotPotatoMovement>().TransferHotPotato(GetComponent<PhotonView>().ownerId);
    }

    protected override void OnPress()
    {
        hasDetonated = true;
        hotPotato.GetComponent<HotPotatoCollision>().Detonate();
        DecreaseCapacity();
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

    private void OnDestroy()
    {
        if (!hasDetonated)
        {
            hotPotato.GetComponent<HotPotatoCollision>().Detonate();
        }
    }
}

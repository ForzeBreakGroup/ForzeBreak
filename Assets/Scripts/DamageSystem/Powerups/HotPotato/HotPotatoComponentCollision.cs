using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotPotatoComponentCollision : PowerUpCollision
{
    private HotPotatoComponent hotPotatoComponent;

    private void Awake()
    {
        hotPotatoComponent = GetComponent<HotPotatoComponent>();
    }

    public override void ComponentCollision(Collision collision)
    {
        // Spawn a hot potato
        GameObject hotpotato = hotPotatoComponent.SpawnHotPotato();
        hotpotato.GetComponent<HotPotatoMovement>().TransferHotPotato(otherCollider.GetComponent<PhotonView>().ownerId);
    }
}

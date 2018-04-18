using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpComponentCollectible : PowerUpCollectible
{
    protected override void PowerUpCollected(PhotonView view)
    {
        base.PowerUpCollected(view);

        // Random generate powerup name
        if (randomMode)
        {
            powerupName = powerUpGrade.GetRandomPowerUp(powerUpTier);
        }

        // Calls to the player owner using RPC to instantiate corresponding powerup from Resources folder
        view.RPC("RemovePowerUpComponent", PhotonTargets.All, view.ownerId);
        view.RPC("AddPowerUpComponent", PhotonTargets.All, powerupName, view.ownerId);

        // Raise a photon event to master client to notify the powerup has been collected, the master client will then appropriately destroy the powerup
        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.MasterClient;
        PhotonNetwork.RaiseEvent((byte)ENetworkEventCode.OnPowerUpCollected, transform.position, true, options);
    }
}

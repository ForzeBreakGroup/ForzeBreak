using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpDataChangeCollectible : PowerUpCollectible
{
    protected override void PowerUpCollected(PhotonView view)
    {
        base.PowerUpCollected(view);

        // Random generate powerup name
        if (randomMode)
        {
            powerupName = powerUpGrade.GetRandomPowerUp(powerUpTier);
        }

        switch(powerupName) 
        {
            case "PowerUp - Health":
                
                DamageSystem ds = NetworkManager.instance.GetLocalPlayer().GetComponent<DamageSystem>();
                // ds.damageAmplifyPercentage = DamageSystem.DamageSystemConstants.baseDamagePercentage;
                break;

            case "PowerUp - Energy":
                BoostControl bc = NetworkManager.instance.GetLocalPlayer().GetComponent<BoostControl>();
                bc.energy = bc.maxEnergy;
                break;
        }


        // Raise a photon event to master client to notify the powerup has been collected, the master client will then appropriately destroy the powerup
        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.MasterClient;
        PhotonNetwork.RaiseEvent((byte)ENetworkEventCode.OnPowerUpCollected, transform.position, true, options);
    }
}

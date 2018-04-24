using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerDeathHandler : NetworkPlayerBase
{
    private GameObject spectCam;
    private int lastDamageReceivedFrom;

    public void OnPlayerDeath()
    {
        int playerNum = 0;
        // Transfer the player camera to spectator
        if (!NetworkManager.offlineMode)
        {
            playerNum = photonView.ownerId;
        }
        NetworkManager.instance.GetPlayerCamera().enabled = false;

        // Activate respawn timer on spectator camera

        // Sends RPC to the killer to increment the kill count

        // Destroy player
    }

    private void Awake()
    {
        // Initialize killer to self
        lastDamageReceivedFrom = photonView.ownerId;

        // Find the spectating camera
        spectCam = GameObject.FindGameObjectWithTag("MainCamera");
    }

    public override void SerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {

        }
        else if (stream.isReading)
        {

        }
    }
}

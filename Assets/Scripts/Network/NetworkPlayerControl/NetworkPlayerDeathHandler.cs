using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerDeathHandler : NetworkPlayerBase
{
    private GameObject spectCam;
    private int lastDamageReceivedFrom;

    public void OnPlayerDeath()
    {
        Debug.Log("OnPlayerDeath");
        int playerNum = 0;
        // Transfer the player camera to spectator
        if (!NetworkManager.offlineMode)
        {
            playerNum = photonView.ownerId;
        }
        NetworkManager.instance.GetPlayerCamera().enabled = false;

        // Activate respawn timer on spectator camera

        // Sends RPC to the killer to increment the kill count
        photonView.RPC("RpcIncrementKillScore", PhotonTargets.All, lastDamageReceivedFrom, photonView.ownerId);

        // Destroy player
        MatchManager.instance.DestroyPlayerObject();
    }

    private void Awake()
    {
        // Initialize killer to self
        lastDamageReceivedFrom = photonView.ownerId;

        // Find the spectating camera
        spectCam = GameObject.FindGameObjectWithTag("MainCamera");
    }

    [PunRPC]
    private void RpcIncrementKillScore(int killerId, int senderId)
    {
        // Suicide does not count
        if (killerId == senderId)
        {
            return;
        }

        if (photonView.ownerId == killerId)
        {
            PhotonPlayer player = PhotonPlayer.Find(photonView.ownerId);
            int updateKillCount = (int)player.CustomProperties["KillCount"] + 1;
            ExitGames.Client.Photon.Hashtable setKillCount = new ExitGames.Client.Photon.Hashtable() { { "KillCount", updateKillCount } };
            player.SetCustomProperties(setKillCount);
        }
    }
}

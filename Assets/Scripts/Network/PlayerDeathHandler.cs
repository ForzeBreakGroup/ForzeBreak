 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathHandler : NetworkPlayerBase
{
    [SerializeField]
    private GameObject spectCam;

    public void OnPlayerDeath(int killerId, int victimId)
    {
        if (NetworkManager.instance.GetLocalPlayer((NetworkManager.offlineMode) ? victimId - 1 : 0) == null)
        {
            return;
        }

        InGameHUDManager.instance.UpdateWeaponIcon("");

        // Transfer the camera to spectator camera
        NetworkManager.instance.GetPlayerCamera().enabled = false;
        Debug.Log("Transferring the player camera to spectator camera");

        // Activate respawn timer on spectator camera
        spectCam.GetComponent<RespawnControl>().StartRespawnTimer(victimId);

        // Suicide does not count
        if (killerId != victimId)
        {
            // Different method of tracking kill score in online and offline mode
            if (NetworkManager.offlineMode)
            {

            }
            else
            {
                // Finds the killer photon player
                PhotonPlayer killer = PhotonPlayer.Find(killerId);

                int updateKillCount = (int)killer.CustomProperties["KillCount"] + 1;
                ExitGames.Client.Photon.Hashtable setKillCount = new ExitGames.Client.Photon.Hashtable() { { "KillCount", updateKillCount } };
                killer.SetCustomProperties(setKillCount);

                Debug.Log("Player #" + killer + " increased kill count to: " + killer.CustomProperties["KillCount"]);
            }
        }

        // Destroy player
        MatchManager.instance.DestroyPlayerObject();
    }
}

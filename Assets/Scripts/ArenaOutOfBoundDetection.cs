using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Arena out of bound detection and calls the life decrement method
 */
public class ArenaOutOfBoundDetection : MonoBehaviour
{
    PlayerDeathHandler deathHandler;

    private void Awake()
    {
        deathHandler = GetComponent<PlayerDeathHandler>();
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject victim = other.transform.root.gameObject;
        if (other.transform.root.tag == "Player" && victim.GetComponent<PhotonView>().isMine)
        {
            int victimId = victim.GetComponent<PhotonView>().ownerId;

            int killerId = ((NetworkPlayerCollision)victim.GetComponent(typeof(NetworkPlayerCollision))).lastReceivedDamageFrom;

            Debug.Log("Player #" + victimId + " is killed by Player #" + killerId);

            deathHandler.OnPlayerDeath(killerId, victimId);

            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX_NonDiegetic/SFX_GameOver");
        }
    }
}

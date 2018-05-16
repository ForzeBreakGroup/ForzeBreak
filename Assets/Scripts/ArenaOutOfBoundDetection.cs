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
    int deathCount = 0;
    int suicideCount = 0;

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
            string killedBy = ((NetworkPlayerCollision)victim.GetComponent(typeof(NetworkPlayerCollision))).receivedDamageItem;

            AnalyticManager.Insert("OnPlayerDeath", other.transform.position, (victimId == killerId), killedBy);

            Debug.Log("Player #" + victimId + " is killed by Player #" + killerId);

            deathHandler.OnPlayerDeath(killerId, victimId);

            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX_NonDiegetic/SFX_GameOver");

            ++deathCount;
            suicideCount = suicideCount + ((victimId == killerId) ? 1 : 0);
        }
    }

    private void OnDestroy()
    {
        AnalyticManager.Insert("PlayerDeathCount", deathCount);
        AnalyticManager.Insert("PlayerSuicideCount", suicideCount);
    }
}

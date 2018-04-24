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
    private void OnTriggerEnter(Collider other)
    {
        PhotonView view = other.transform.root.gameObject.GetPhotonView();
        if (other.transform.root.tag == "Player" && view.isMine)
        {
            int playerId = view.ownerId;

            other.transform.root.gameObject.GetComponent<NetworkPlayerDeathHandler>().OnPlayerDeath();

            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX_NonDiegetic/SFX_GameOver");
        }

    }

}

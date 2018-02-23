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
        if (other.transform.root.tag == "Player")
        {
            MatchManager.instance.DestroyPlayerObject();

            int playerId = other.transform.root.gameObject.GetPhotonView().ownerId;

            RaiseEventOptions options = new RaiseEventOptions();
            options.Receivers = ReceiverGroup.MasterClient;
            PhotonNetwork.RaiseEvent((int)ENetworkEventCode.OnPlayerDeath, playerId, true, options);
        }
    }
}

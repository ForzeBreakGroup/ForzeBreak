using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Network synchronized script to manage client player's life during match.
 * Calls to host to change scene once a player's life has been depleted.
 */
public class PlayerLife : NetworkPlayerData
{
    #region Private Members
    /// <summary>
    ///  Player's remaining life synchronized and managed by server/host
    /// </summary>
    [SerializeField] private int playerLife = 3;

    /// <summary>
    /// Networked Command method to request the host to change the scene.
    /// </summary>
    private void CmdPlayerLifeDepleted()
    {
        RaiseEventOptions evtOptions = new RaiseEventOptions();
        evtOptions.Receivers = ReceiverGroup.MasterClient;
        PhotonNetwork.RaiseEvent((byte)ENetworkEventCode.OnPlayerDeath, null, true, evtOptions);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Invoked by server to ask specific client to respawn the player object
    /// </summary>
    /// <param name="target"> NetworkConnection ID used to identify client </param>
    private void TargetRespawn()
{
        // Reset physic rigidbody
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        // Reset transform
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;
    }
    #endregion

    /// <summary>
    /// Used by local player object to decrease the current player's life and reflect to server
    /// </summary>
    public void DecrementPlayerLife()
    {
        // Decrements the current player life
        --playerLife;

        if (playerLife <= 0)
        {
            // Calls to server if player's life reached threshold
            CmdPlayerLifeDepleted();
        }
        else
        {
            // Calls respawn the player
            TargetRespawn();
        }
    }
}

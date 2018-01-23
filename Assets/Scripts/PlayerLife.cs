using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Network synchronized script to manage client player's life during match.
 * Calls to host to change scene once a player's life has been depleted.
 */
public class PlayerLife : NetworkBehaviour
{
    #region Private Members
    /// <summary>
    ///  Player's remaining life synchronized and managed by server/host
    /// </summary>
    [SyncVar] private int playerLife = 3;

    /// <summary>
    /// Networked Command method to request the host to change the scene.
    /// </summary>
    [Command]
    private void CmdPlayerLifeDepleted()
    {
        // Change scene
        NetworkHandler.singleton.ServerChangeScene("End");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Invoked by server to ask specific client to respawn the player object
    /// </summary>
    /// <param name="target"> NetworkConnection ID used to identify client </param>
    [TargetRpc]
    private void TargetRespawn(NetworkConnection target)
    {
        if (isLocalPlayer)
        {
            this.gameObject.transform.position = NetworkHandler.singleton.startPositions[0].position;
        }
    }
    #endregion

    /// <summary>
    /// Used by local player object to decrease the current player's life and reflect to server
    /// </summary>
    public void DecrementPlayerLife()
    {
        if (!isServer)
            return;

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
            TargetRespawn(connectionToClient);
        }

        Debug.Log(playerLife);
    }
}

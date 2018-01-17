using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Record of player's current life point with public method for other player scripts to access to decrement it.
 * Once the life point reached zero (0), it calls to the server to notify player state change via Command.
 */
public class PlayerLife : NetworkBehaviour
{
    [SyncVar]
    private int playerLife = 3;
 
    [Command]
    private void CmdPlayerLifeDepleted()
    {
        // Change scene
        NetworkHandler.singleton.ServerChangeScene("End");
    }

    // Public method for local player to decrease number of life
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

    [TargetRpc]
    void TargetRespawn(NetworkConnection target)
    {
        if (isLocalPlayer)
        {
            NetworkConnection conn = connectionToClient;
            GameObject newPlayer = Instantiate<GameObject>(this.gameObject, NetworkHandler.instance.startPositions[0].position, Quaternion.identity);
            NetworkServer.ReplacePlayerForConnection(conn, newPlayer, 0);
            Destroy(this.gameObject);
        }
    }
}

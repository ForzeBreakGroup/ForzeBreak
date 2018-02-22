using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Base class for handling the player metadata required to transfer between network and shares to all other clients
 */
public class NetworkPlayerData : NetworkPlayerBase
{
    public Camera localCam;

    #region Private Members
    /// <summary>
    ///  Player's remaining life synchronized and managed by server/host
    /// </summary>
    [SerializeField] private int playerLife = 3;

    /// <summary>
    /// Player's spawning position given by matchmanager from host
    /// </summary>
    private Vector3 spawnPosition;

    /// <summary>
    /// Player's spawning rotation given by matchmanager from host
    /// </summary>
    private Quaternion spawnRotation;
    #endregion

    #region Private Methods
    private void Start()
    {
        spawnPosition = Vector3.zero;
        spawnRotation = Quaternion.identity;
    }

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

    /// <summary>
    /// Networked Command method to request the host to change the scene.
    /// </summary>
    private void CmdPlayerLifeDepleted()
    {
        RaiseEventOptions evtOptions = new RaiseEventOptions();
        evtOptions.Receivers = ReceiverGroup.MasterClient;
        PhotonNetwork.RaiseEvent((byte)ENetworkEventCode.OnPlayerDeath, null, true, evtOptions);
    }

    /// <summary>
    /// Photon SerializeView method for synchronizing data between all clients
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="info"></param>
    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        this.SerializeView(stream, info);

        NetworkPlayerInput.SerializeView(stream, info);
        NetworkPlayerMovement.SerializeView(stream, info);
        NetworkPlayerVisual.SerializeView(stream, info);
        NetworkPlayerCollision.SerializeView(stream, info);
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

    /// <summary>
    /// Registers the spawn information of the player
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    public void RegisterSpawnInformation(Vector3 pos, Quaternion rot)
    {
        spawnPosition = pos;
        spawnRotation = rot;
    }
}

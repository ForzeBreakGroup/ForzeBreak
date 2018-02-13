using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Base class for NetworkPlayers, this obtains all other possible NetworkPlayer components for cross-referencing easier
 * if the component derives from this class, using singleton pattern for all components
 */
public class NetworkPlayerBase : Photon.MonoBehaviour
{
    /// <summary>
    /// Internal reference to NetworkPlayerInput
    /// </summary>
    private NetworkPlayerInput playerInput;
    protected NetworkPlayerInput NetworkPlayerInput
    {
        get
        {
            if (!playerInput)
            {
                playerInput = GetComponent(typeof(NetworkPlayerInput)) as NetworkPlayerInput;
                if (!playerInput)
                {
                    Debug.LogError("NetworkPlayerInput Script must be attached to Player Object");
                }
            }
            return playerInput;
        }
    }

    /// <summary>
    /// Internal reference to NetworkPlayerVisual
    /// </summary>
    private NetworkPlayerVisual playerVisual;
    protected NetworkPlayerVisual NetworkPlayerVisual
    {
        get
        {
            if (!playerVisual)
            {
                playerVisual = GetComponent(typeof(NetworkPlayerVisual)) as NetworkPlayerVisual;
                if (!playerVisual)
                {
                    Debug.LogError("NetworkPlayerVisual Script must be attached to Player Object");
                }
            }
            return playerVisual;
        }
    }

    /// <summary>
    /// Internal reference to NetworkPlayerMovement
    /// </summary>
    private NetworkPlayerMovement playerMovement;
    protected NetworkPlayerMovement NetworkPlayerMovement
    {
        get
        {
            if (!playerMovement)
            {
                playerMovement = GetComponent(typeof(NetworkPlayerMovement)) as NetworkPlayerMovement;
                if (!playerMovement)
                {
                    Debug.LogError("NetworkPlayerMovement Script must be attached to Player Object");
                }
            }
            return playerMovement;
        }
    }

    /// <summary>
    /// Internal reference to NetworkPlayerData
    /// </summary>
    private NetworkPlayerData playerData;
    protected NetworkPlayerData NetworkPlayerData
    {
        get
        {
            if (!playerData)
            {
                playerData = GetComponent(typeof(NetworkPlayerData)) as NetworkPlayerData;
                if (!playerData)
                {
                    Debug.LogError("NetworkPlayerData Script must be attached to Player Object");
                }
            }
            return playerData;
        }
    }

    /// <summary>
    /// Internal reference to NetworkPlayerCollision
    /// </summary>
    private NetworkPlayerCollision playerCollision;
    protected NetworkPlayerCollision NetworkPlayerCollision
    {
        get
        {
            if (!playerCollision)
            {
                playerCollision = GetComponent(typeof(NetworkPlayerCollision)) as NetworkPlayerCollision;
                if (!playerCollision)
                {
                    Debug.LogError("NetworkPlayerCollision Script must be attached to Player Object");
                }
            }
            return playerCollision;
        }
    }

    /// <summary>
    /// Virtual method for send/receive photon data
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="info"></param>
    public virtual void SerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        return;
    }
}

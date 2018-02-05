using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class NetworkPlayerBase : Photon.MonoBehaviour
{
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


    public virtual void SerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        return;
    }
}

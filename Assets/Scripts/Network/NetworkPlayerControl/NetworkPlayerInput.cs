using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Serves as base framework for transmitting player input across the network
 */
public class NetworkPlayerInput : NetworkPlayerBase
{
    /// <summary>
    /// Enables the input script only on local player
    /// </summary>
    protected virtual void Awake()
    {
        //enabled = photonView.isMine;
    }

    private void Update()
    {
        if (photonView.isMine)
        {
            PlayerInputUpdate();
        }
    }

    private void FixedUpdate()
    {
        if (photonView.isMine)
        {
            PlayerInputUpdate();
        }
    }

    /// <summary>
    /// Virtual method to override by child class for update callback
    /// </summary>
    protected virtual void PlayerInputUpdate()
    {

    }

    /// <summary>
    /// Virtual method to override by child class for input reading
    /// </summary>
    protected virtual void PlayerInputFixedUpdate()
    {
    }
}

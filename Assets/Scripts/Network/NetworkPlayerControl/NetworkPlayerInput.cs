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
    private void Awake()
    {
        enabled = photonView.isMine;
    }

    private void FixedUpdate()
    {
        PlayerInputUpdate();
    }

    /// <summary>
    /// Virtual method to override by child class for input reading
    /// </summary>
    protected virtual void PlayerInputUpdate()
    {
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerInput : NetworkPlayerBase
{
    private void Awake()
    {
        enabled = photonView.isMine;
    }

    private void FixedUpdate()
    {
        PlayerInputUpdate();
    }

    protected virtual void PlayerInputUpdate()
    {
    }
}

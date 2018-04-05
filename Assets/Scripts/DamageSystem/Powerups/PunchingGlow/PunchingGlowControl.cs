using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Carry
 * 
 * Description:
 * Control class of Punching Glow.
 */

public class PunchingGlowControl : PowerUpBase
{

    // Use this for initialization
    private void Awake()
    {
        this.enabled = photonView.isMine;
    }

    public override void AdjustModel()
    {
        base.AdjustModel();
    }

    protected override void OnPress()
    {
        
    }
}

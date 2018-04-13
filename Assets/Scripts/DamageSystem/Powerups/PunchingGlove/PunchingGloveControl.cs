using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Carry
 * 
 * Description:
 * Control class of Punching Glow.
 */

public class PunchingGloveControl : PowerUpBase
{
    private Transform bulletSpawn;
    // Use this for initialization
    private void Awake()
    {
        this.enabled = photonView.isMine;
        bulletSpawn = transform.Find("GlovePos");
    }

    public override void AdjustModel()
    {
        base.AdjustModel();
    }
     
    protected override void OnPress()
    {
        Debug.Log("trans pos  " + transform.position);
        Debug.Log("trans rotation  " + transform.rotation);
        GameObject glove = PhotonNetwork.Instantiate("PunchingGloveOnly", transform.position, transform.rotation, 0);
        PhotonNetwork.Destroy(gameObject);
    }
}

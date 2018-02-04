using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerInput : NetworkPlayerBase
{
    public Camera cam;
    public float horizontalAxis = 0;
    public float verticalAxis = 0;

    private void Start()
    {
        enabled = photonView.isMine;

        if (photonView.isMine)
        {
            Instantiate(cam).gameObject.GetComponent<PlayerCamera>().target = this.gameObject;
        }
    }

    private void Update()
    {
        horizontalAxis = Input.GetAxis("Horizontal");
        verticalAxis = Input.GetAxis("Vertical");
    }

    public override void SerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.SerializeView(stream, info);
    }
}

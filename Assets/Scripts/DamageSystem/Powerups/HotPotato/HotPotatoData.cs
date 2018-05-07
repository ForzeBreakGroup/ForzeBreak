using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotPotatoData : PowerUpData
{
    [SerializeField]
    private float detonateTimer = 30.0f;

    private void Update()
    {
        if (photonView.isMine)
        {
            detonateTimer -= Time.deltaTime;

            if (detonateTimer <= 0)
            {
                ((HotPotatoCollision)PowerUpCollision).Detonate();
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(detonateTimer);
        }
        else if (stream.isReading)
        {
            detonateTimer = (float)stream.ReceiveNext();
        }
    }
}

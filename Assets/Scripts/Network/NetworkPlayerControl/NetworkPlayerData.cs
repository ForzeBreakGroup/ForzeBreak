using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerData : NetworkPlayerBase
{
    [SerializeField] int health = 100;

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        this.SerializeView(stream, info);
        NetworkPlayerInput.SerializeView(stream, info);
        NetworkPlayerMovement.SerializeView(stream, info);
        NetworkPlayerVisual.SerializeView(stream, info);
    }

    public override void SerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(health);
        }
        else if (stream.isReading)
        {
            health = (int)stream.ReceiveNext();
        }
    }
}

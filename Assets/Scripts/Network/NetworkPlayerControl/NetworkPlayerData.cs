using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerData : NetworkPlayerBase
{
    [SerializeField] protected Vector3 spawnPosition;
    [SerializeField] protected Quaternion spawnRotation;

    private void Start()
    {
        spawnPosition = Vector3.zero;
        spawnRotation = Quaternion.identity;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        this.SerializeView(stream, info);

        NetworkPlayerInput.SerializeView(stream, info);
        NetworkPlayerMovement.SerializeView(stream, info);
        NetworkPlayerVisual.SerializeView(stream, info);
        NetworkPlayerCollision.SerializeView(stream, info);
    }

    public void RegisterSpawnInformation(Vector3 pos, Quaternion rot)
    {
        spawnPosition = pos;
        spawnRotation = rot;
    }
}

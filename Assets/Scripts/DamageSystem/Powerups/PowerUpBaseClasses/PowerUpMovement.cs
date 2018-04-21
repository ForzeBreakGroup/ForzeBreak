using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpMovement : PowerUpProjectileBase
{
    protected virtual void OnDestroy()
    {

    }

    public void DestroyPowerUpProjectile()
    {
        PhotonView.RPC("RpcDestroyPowerUpProjectile", PhotonTargets.All);
    }

    [PunRPC]
    public virtual void RpcDestroyPowerUpProjectile()
    {
        if (PhotonView.isMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}

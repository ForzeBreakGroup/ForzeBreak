using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpProjectileBase : Photon.MonoBehaviour
{
    private PowerUpData powerupData;
    protected PowerUpData PowerUpData
    {
        get
        {
            if (!powerupData)
            {
                powerupData = GetComponent(typeof(PowerUpData)) as PowerUpData;
                if (!powerupData)
                {
                    Debug.LogError("PowerUpData component is not attached to the GameObject");
                }
            }

            return powerupData;
        }
    }

    private PowerUpMovement powerupMovement;
    protected PowerUpMovement PowerUpMovement
    {
        get
        {
            if (!powerupMovement)
            {
                powerupMovement = GetComponent(typeof(PowerUpMovement)) as PowerUpMovement;
                if (!powerupMovement)
                {
                    Debug.LogError("PowerUpMovement component is not attached to the GameObject");
                }
            }

            return powerupMovement;
        }
    }

    private PowerUpCollision powerupCollision;
    protected PowerUpCollision PowerUpCollision
    {
        get
        {
            if (!powerupCollision)
            {
                powerupCollision = GetComponent(typeof(PowerUpCollision)) as PowerUpCollision;
                if (!powerupCollision)
                {
                    Debug.LogError("PowerUpCollision Component is not attached to the GameObject");
                }
            }

            return powerupCollision;
        }
    }

    private PhotonView photonView;
    protected PhotonView PhotonView
    {
        get
        {
            if (!photonView)
            {
                photonView = GetComponent(typeof(PhotonView)) as PhotonView;
                if (!photonView)
                {
                    Debug.LogError("PhotonView component is not attached to the GameObject");
                }
            }

            return photonView;
        }
    }
}

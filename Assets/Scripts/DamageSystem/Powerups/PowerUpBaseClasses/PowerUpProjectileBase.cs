using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PowerUpMovement))]
[RequireComponent(typeof(PowerUpData))]
[RequireComponent(typeof(PowerUpCollision))]
public class PowerUpProjectileBase : MonoBehaviour
{
    protected PowerUpMovement powerupMovement;
    protected PowerUpCollision powerupCollision;
    protected PowerUpData powerupData;

    private void Awake()
    {
        powerupMovement = GetComponent<PowerUpMovement>();
        powerupData = GetComponent<PowerUpData>();
        powerupCollision = GetComponent<PowerUpCollision>();
    }
}

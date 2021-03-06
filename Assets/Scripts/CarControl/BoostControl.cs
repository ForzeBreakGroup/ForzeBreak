﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Author: Robin
 * 
 * Description:
 * Control the visual effect of the boost flame
 */
public class BoostControl : MonoBehaviour {

    public float boostPower = 30.0f;
    public float energy = 100.0f;
    public float boostMaxSpeed = 25.0f;
    public float energyDecay = 0.5f;
    public float energyRecover = 0.01f;
    public float maxEnergy = 100.0f;


    private BoostEffectControl boostEffect;
    private Rigidbody carRigidbody;
    private CarControlWheels carController;
    private CarUserControl carControlInput;

    private void Awake()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carController = GetComponent<CarControlWheels>();
        boostEffect = GetComponentInChildren<BoostEffectControl>();
        carControlInput = GetComponent<CarUserControl>();
    }

    private void LateUpdate()
    {
        if (carControlInput.boost)
        {
            Boost();
        }
        else
        {
            Recover();
        }
        if(GetComponent<PhotonView>().isMine)
            InGameHUDManager.instance.UpdateBoostBar(energy / maxEnergy);
    }

    public void Boost()
    {
        if ((int)energy > 1)
        {
            carController.ApplyDrive(1,0);
            carController.IsBoosting = true;
            carController.MaxSpeed = boostMaxSpeed;
            carRigidbody.AddForce(transform.forward * boostPower, ForceMode.Acceleration);
            energy = energy < 0 ? 0 : energy - energyDecay;

            if (boostEffect != null)
            {
                boostEffect.UpdateColorBySpeed(true);
            }
        }
        else
        {
            Recover();
        }
    }

    public void Recover()
    {
        carController.IsBoosting = false;

        if (energy < maxEnergy)
            energy += energyRecover;

        if (boostEffect != null)
        {
            boostEffect.UpdateColorBySpeed(false);
        }
    }

}

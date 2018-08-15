using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Author: Robin
 * 
 * Description:
 * Control the visual effect of the boost flame
 */
public class BoostControl : MonoBehaviour {

    enum BoostType { Up, Forward };

    [SerializeField] private BoostType boostType = BoostType.Forward;
    public float boostPower = 30.0f;
    public float energy = 100.0f;
    public float boostMaxSpeed = 25.0f;
    public float energyDecay = 0.5f;
    public float energyRecover = 0.01f;
    public float maxEnergy = 100.0f;

    private float soundCD = 1f;
    private float nextplayTime = 0f;
    private bool canPlaySound = true;


    private BoostEffectControl[] boostEffects;
    private Rigidbody carRigidbody;
    private CarControlWheels carController;
    private CarUserControl carControlInput;

    private void Awake()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carController = GetComponent<CarControlWheels>();
        carControlInput = GetComponent<CarUserControl>();

        boostEffects = transform.GetComponentsInChildren<BoostEffectControl>();
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
            if(boostType==BoostType.Forward)
                carRigidbody.AddForce(transform.forward * boostPower, ForceMode.Acceleration);
            else if(boostType == BoostType.Up)
            {
                carRigidbody.AddForce(transform.up * boostPower, ForceMode.Acceleration);
                carRigidbody.AddTorque(Vector3.Cross(transform.up, Vector3.up) * (Vector3.Angle(transform.up, Vector3.up)), ForceMode.Acceleration);
            }
            energy = energy < 0 ? 0 : energy - energyDecay;

            if (boostEffects.Length != 0)
            {
                foreach(BoostEffectControl bec in boostEffects)
                {
                    bec.UpdateColorBySpeed(true);
                }
            }
            if (canPlaySound && Time.time > nextplayTime)
            {
                nextplayTime = Time.time + soundCD;
                canPlaySound = false;
                GetComponent<VehicleSound>().PlayBoostSound();
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

        canPlaySound = true;

        if (boostEffects.Length != 0)
        {
            foreach (BoostEffectControl bec in boostEffects)
            {
                bec.UpdateColorBySpeed(false);
            }
        }
    }

}

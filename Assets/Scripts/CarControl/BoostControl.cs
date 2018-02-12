using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostControl : MonoBehaviour {

    public float boostPower = 30.0f;
    public float energy = 100.0f;
    public float boostMaxSpeed = 50.0f;
    public float energyDecay = 0.5f;
    public float energyRecover = 1.0f;
    public float maxEnergy = 100.0f;



    private Rigidbody carRigidbody;
    private CarControlWheels carController;

    private void Awake()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carController = GetComponent<CarControlWheels>();
    }

    public void Boost()
    {
        if (carController.IsWheelsGround&&energy>0)
        {
            carController.IsBoosting = true;
            carController.MaxSpeed = boostMaxSpeed;
            carRigidbody.AddForce(transform.forward*boostPower, ForceMode.Acceleration);
            energy = energy < 0 ? 0 : energy - energyDecay;
        }
    }
    public void Recover()
    {
        carController.IsBoosting = false;

        if (energy< maxEnergy)
            energy += energyRecover;
    }

}

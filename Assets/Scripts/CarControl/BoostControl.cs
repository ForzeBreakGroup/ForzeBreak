using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostControl : MonoBehaviour {

    [SerializeField] private float boostPower = 30.0f;
    [SerializeField] private float energy = 100.0f;
    [SerializeField] private float boostMaxSpeed = 80.0f;
    [SerializeField] private float energyDecay = 0.5f;
    [SerializeField] private float energyRecover = 1.0f;
    [SerializeField] private float maxEnergy = 100.0f;



    private Rigidbody carRigidbody;
    private CarControlWheels carController;

    private void Start()
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

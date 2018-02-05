using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipControl : MonoBehaviour {

    private Rigidbody carRigidbody;
    private CarControlWheels carController;
    private bool canFlip = true;
    private bool carBodyGrounded = false;
    [SerializeField] private float flipCD = 0.2f;
    [SerializeField] private float upForce_wheelsGrounded = 7500f;
    [SerializeField] private float upForce_overturned = 3500f;
    [SerializeField] private float sideForce = 1000f;

    private float nextFlip = 0.0f;

    private void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carController = GetComponent<CarControlWheels>();
    }

    public void Flip(bool flipInput, float dir)
    {
        if (Time.time>nextFlip)
        {
            canFlip = true;
        }
        
        if (flipInput == false)
            return;

        if(canFlip)
        {
            if (carController.IsWheelsGround)
            {
                Debug.Log("aaa");
                canFlip = false;
                nextFlip = Time.time + flipCD;

                carRigidbody.AddForce(transform.up * upForce_wheelsGrounded, ForceMode.Impulse);

                if(dir>0)
                    carRigidbody.AddRelativeTorque(-Vector3.forward * sideForce, ForceMode.Acceleration);
                else
                    carRigidbody.AddRelativeTorque(Vector3.forward * sideForce, ForceMode.Acceleration);

            }
            else if(transform.up.y<0&&carBodyGrounded)
            {
                canFlip = false;
                nextFlip = Time.time + flipCD;
                
                carRigidbody.AddForce(Vector3.up * upForce_overturned, ForceMode.Impulse);
                if (dir > 0)
                    carRigidbody.AddRelativeTorque(-transform.forward * sideForce, ForceMode.Acceleration);
                else
                    carRigidbody.AddRelativeTorque(Vector3.forward * sideForce, ForceMode.Acceleration);
            }


        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        carBodyGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        carBodyGrounded = false;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipControl : MonoBehaviour {
    
    private Rigidbody carRigidbody;
    private CarControlWheels carController;
    private bool canFlip = true;

    private void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carController = GetComponent<CarControlWheels>();
    }

    public void Flip()
    {
        if (!carController.IsWheelsGround && transform.up.y<0&& canFlip)
        {
            canFlip = false;
            carRigidbody.AddForce(Vector3.up * 2500, ForceMode.Impulse);
            carRigidbody.AddRelativeTorque(Vector3.forward * 1000, ForceMode.Acceleration);
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("enter");
        canFlip = true;
    }
    //private bool IsGrounded()
    //{
    //    return Physics.Raycast(transform.position, -Vector3.up, 1);
    //}


}

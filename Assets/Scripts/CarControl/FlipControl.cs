using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Robin Zhou
 * 
 * Description:
 * Flip control class for the car.
 * 
 */
public class FlipControl : MonoBehaviour {

    private Rigidbody carRigidbody;
    private CarControlWheels carController;
    /// <summary>
    /// time duration for next flip
    /// </summary>
    [SerializeField] private float flipCD = 0.2f;
    /// <summary>
    /// velocity change when wheels of the car is grounded.
    /// </summary>
    [SerializeField] private float upForce_wheelsGrounded = 5f;
    /// <summary>
    /// velocity change when wheels of the car is upside down.
    /// </summary>
    [SerializeField] private float upForce_overturned = 5f;
    /// <summary>
    /// side force applied when car is flipping. related to rotating speed.
    /// </summary>
    [SerializeField] private float sideForce = 1000f;

    private bool canFlip = true;
    private bool carBodyGrounded = false;
    private float nextFlip = 0.0f;

    //flipsound
    private FMOD.Studio.EventInstance flipSound;

    private void Awake()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carController = GetComponent<CarControlWheels>();

        flipSound = FMODUnity.RuntimeManager.CreateInstance("event:/SFX_Diegetic/SFX_VehicleFlip");
    }

    /// <summary>
    /// Flip function, call it every update.
    /// </summary>
    /// <param name="flipInput">if the input want to flip</param>
    /// <param name="dir">flip direction, + for right, - for left</param>
    public void Flip(bool flipInput, float dir)
    {
        //CD
        if (Time.time > nextFlip)
        {
            canFlip = true;
        }

        //return
        if (flipInput == false)
            return;

        if (canFlip)
        {
            //grounded condition
            if (carController.IsAnyWheelGround)
            {
                canFlip = false;
                nextFlip = Time.time + flipCD;

                FMODUnity.RuntimeManager.AttachInstanceToGameObject(flipSound, transform, GetComponent<Rigidbody>());
                flipSound.start();

                //StartCoroutine(FlipRotate(2,dir));
                carRigidbody.AddForce(transform.up * upForce_wheelsGrounded, ForceMode.VelocityChange);
                if (dir > 0)
                    carRigidbody.AddRelativeTorque(-Vector3.forward * sideForce, ForceMode.Acceleration);
                else
                    carRigidbody.AddRelativeTorque(Vector3.forward * sideForce, ForceMode.Acceleration);

            }
            //upside down condition
            else if (transform.up.y < 0.1f && carBodyGrounded)
            {
                canFlip = false;
                nextFlip = Time.time + flipCD;


                carRigidbody.AddForce(Vector3.up * upForce_overturned, ForceMode.VelocityChange);
                if (dir > 0)
                    carRigidbody.AddRelativeTorque(-Vector3.forward * 1000f, ForceMode.Acceleration);
                else
                    carRigidbody.AddRelativeTorque(Vector3.forward * 1000f, ForceMode.Acceleration);
            }
        }

    }

    //update the state of carbody.

    private void OnCollisionStay(Collision collision)
    {
        carBodyGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        carBodyGrounded = false;
    }



    //IEnumerator FlipRotate(float duration, float dir)
    //{
    //    float startRotation = transform.eulerAngles.z;
    //    Debug.Log(startRotation);

    //    float endRotation = 360f;
    //    if (dir > 0)
    //        endRotation = 360f + startRotation;
    //    else
    //        endRotation = 360f - startRotation;

    //    float t = 0.0f;
    //    while (t < duration)
    //    {
    //        t += Time.deltaTime;
    //        startRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
    //        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, startRotation);
    //        yield return null;
    //    }
    //}
}

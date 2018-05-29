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

    [SerializeField] private bool selfAdjust = true;

    [SerializeField] private float selfAdjustDelay = 0.5f;
    [SerializeField] private float selfAdjustDetectDistance = 2f;
    private bool canFlip = true;

    private float nextFlip = 0.0f;


    private void Awake()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carController = GetComponent<CarControlWheels>();
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

                VehicleSound vs = GetComponent<VehicleSound>();
                if (vs != null)
                    FMODUnity.RuntimeManager.PlayOneShot(vs.flipSoundref, transform.position);

                if(selfAdjust)
                    StartCoroutine(SelfRotationControl());
                carRigidbody.AddForce(transform.up * (upForce_wheelsGrounded + 100f * carRigidbody.velocity.magnitude), ForceMode.Impulse);
                
                if (dir > 0)
                    carRigidbody.AddRelativeTorque(-Vector3.forward * sideForce, ForceMode.Acceleration);
                else
                    carRigidbody.AddRelativeTorque(Vector3.forward * sideForce, ForceMode.Acceleration);

                carRigidbody.angularVelocity = new Vector3(carRigidbody.angularVelocity.x, 0, carRigidbody.angularVelocity.z);

            }
            //upside down condition
            else if (transform.up.y < 0.1f && carController.IsCarBodyGround)
            {
                canFlip = false;
                nextFlip = Time.time + flipCD;


                carRigidbody.AddForce(Vector3.up * upForce_overturned, ForceMode.VelocityChange);
                StartCoroutine(SelfRotationControl());

            }
        }

    }

    IEnumerator SelfRotationControl()
    {
        yield return new WaitForSeconds(selfAdjustDelay);

        while(true)
        {
            yield return null;

            if (!carController.IsAnyWheelGround)
            {
                RaycastHit hit;
                Physics.Raycast(transform.position, Vector3.down, out hit, selfAdjustDetectDistance, 1 << 18);

                if(hit.transform != null)
                {
                    //transform.Rotate(-Vector3.Cross(hit.normal, transform.up), Vector3.Angle(hit.normal, transform.up), Space.Self);
                    //transform.rotation = Quaternion.Lerp(transform.rotation, Quater)

                    //transform.Rotate(transform.worldToLocalMatrix * transform.forward, Vector3.Angle(Vector3.Cross(hit.normal, transform.forward), transform.right));
                    //carRigidbody.angularVelocity = Vector3.zero;
                    //transform.Rotate(transform.worldToLocalMatrix * transform.forward, (Vector3.Angle(transform.right, hit.normal) - 90f)*0.3f, Space.Self);
                    //transform.Rotate(transform.worldToLocalMatrix * -transform.right, (Vector3.Angle(transform.forward, hit.normal) - 90f)*0.3f, Space.Self);
                    //transform.Rotate(transform.worldToLocalMatrix * Vector3.Cross(transform.up, hit.normal), Vector3.Angle(transform.up, hit.normal) * 0.2f, Space.Self);

                    carRigidbody.AddTorque(Vector3.Cross(transform.up, Vector3.up) * (Vector3.Angle(transform.up, Vector3.up))*10f, ForceMode.Acceleration);
                }
            }
            else
            {
                break;
            }

        }

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

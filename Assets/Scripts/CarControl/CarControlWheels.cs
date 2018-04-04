using System;
using UnityEngine;

/*
 * Author: Robin Zhou
 * 
 * Description:
 * Car Control Class, handle car moving related stuff.
 * 
 */
public class CarControlWheels : NetworkPlayerMovement
{
    /// <summary>
    /// Actual wheel models, ordered by Front right->Front left->Rear right->Rear left.
    /// </summary>
    [SerializeField] private GameObject[] wheelMeshes = new GameObject[4];

    /// <summary>
    /// wheel colliders, ordered by Front right->Front left->Rear right->Rear left.
    /// </summary>
    [SerializeField] private WheelCollider[] wheelColliders = new WheelCollider[4];
    /// <summary>
    /// Mass offset of All wheels.
    /// </summary>
    [SerializeField] private Vector3 centreOfMassOffset;
    [SerializeField] private float maximumSteerAngle =30.0f;
    /// <summary>
    /// add a helper for steering when fast speed, 0 is realistic physics , 1 the car will grip in the direction it is facing
    /// </summary>
    [Range(0, 1)] [SerializeField] private float steerHelper; 
    /// <summary>
    /// Forward torque
    /// </summary>
    [SerializeField] private float fullTorqueOverAllWheels = 30f;
    /// <summary>
    /// Backward torque
    /// </summary>
    [SerializeField] private float reverseTorque= 20f;

    /// <summary>
    /// Applying a down force related to speed.
    /// </summary>
    [SerializeField] private float downforce = 100f;
    /// <summary>
    /// max speed for car
    /// </summary>
    [SerializeField] private float topspeed = 15;


    private Quaternion[] wheelMeshLocalRotations;
    private float steerAngle;
    private float oldRotation;
    private float currentTorque;
    private Rigidbody carRigidbody;
    
    public bool IsBoosting { get; set; }
    public bool IsWheelsGround { get; set; }
    public bool IsAnyWheelGround { get; set; }
    public float BrakeInput { get; private set; }
    public float CurrentSteerAngle{ get { return steerAngle; }}
    public float CurrentSpeed{ get { return carRigidbody.velocity.magnitude; }}
    public float MaxSpeed { get; set; }
    public float Revs { get; private set; }
    public float AccelInput { get; private set; }

    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();

        wheelMeshLocalRotations = new Quaternion[4];
        for (int i = 0; i < 4; i++)
        {
            wheelMeshLocalRotations[i] = wheelMeshes[i].transform.localRotation;
        }
        wheelColliders[0].attachedRigidbody.centerOfMass = centreOfMassOffset;
        

        carRigidbody = GetComponent<Rigidbody>();
        currentTorque = fullTorqueOverAllWheels;

        MaxSpeed = topspeed;

        IsBoosting = false;
    }

    

    /// <summary>
    /// Car moving function
    /// </summary>
    /// <param name="steering">Steering input, + for right, - for left</param>
    /// <param name="accel">Accelerate input£¬ + for forward, - for backward</param>
    /// <param name="footbrake">foot brake input, - for brake</param>
    public void Move(float steering, float accel, float footbrake)
    {

        IsWheelsGround = true;
        IsAnyWheelGround = false;
        //apply new position and rotation for wheel models. and update two wheel ground parameters.
        for (int i = 0; i < 4; i++)
        {
            Quaternion quat;
            Vector3 position;
            wheelColliders[i].GetWorldPose(out position, out quat);
            wheelMeshes[i].transform.position = position;
            wheelMeshes[i].transform.rotation = quat;

            WheelHit wheelhit;
            wheelColliders[i].GetGroundHit(out wheelhit);
            if (wheelhit.normal == Vector3.zero)
                IsWheelsGround = false;
            else
                IsAnyWheelGround = true;
        }


        //clamp input values
        steering = Mathf.Clamp(steering, -1, 1);
        AccelInput = accel = Mathf.Clamp(accel, 0, 1);
        BrakeInput = footbrake = -1*Mathf.Clamp(footbrake, -1, 0);

        //Set the steer on the front wheels.
        //Assuming that wheels 0 and 1 are the front wheels.
        steerAngle = steering*maximumSteerAngle;
        wheelColliders[0].steerAngle = steerAngle;
        wheelColliders[1].steerAngle = steerAngle;

        SteerHelper();
        ApplyDrive(accel, footbrake);
        CapSpeed();
        
        AddDownForce();
        //CheckForWheelSpin();
    }

    /// <summary>
    /// clamp speed to maxspeed.
    /// </summary>
    private void CapSpeed()
    {
        if (!IsBoosting && MaxSpeed > topspeed)
            MaxSpeed = Mathf.Lerp(MaxSpeed, topspeed, 0.2f);



        float speed = carRigidbody.velocity.magnitude;
        //if moving backward, apply 1/4 speed
        if(Vector3.Angle(transform.forward, carRigidbody.velocity) > 150f)
        {
            if (speed > MaxSpeed / 4)
                carRigidbody.velocity = Vector3.Lerp(carRigidbody.velocity, MaxSpeed / 4 * carRigidbody.velocity.normalized,0.6f);
        }
        else
        {
            if (speed > MaxSpeed)
                carRigidbody.velocity = Vector3.Lerp(carRigidbody.velocity, MaxSpeed * carRigidbody.velocity.normalized, 0.6f);
        }

    }

    /// <summary>
    /// Apply drive to the car
    /// </summary>
    /// <param name="accel">Accelerate input£¬ + for forward, - for backward</param>
    /// <param name="footbrake">foot brake input, - for brake</param>
    public void ApplyDrive(float accel, float footbrake)
    {
        //if no input, stop the car gradually
        if(accel<0.01f&&footbrake<0.01f&&accel>-0.01f&&footbrake>-0.01f&&IsWheelsGround)
        {
            carRigidbody.velocity = Vector3.Lerp(carRigidbody.velocity, Vector3.zero, 0.01f);
        }

        //if moving backward, stop first by lerp then apply forward force
        if (CurrentSpeed > 0.1f && Vector3.Angle(transform.forward, carRigidbody.velocity) > 170f)
        {
            if (IsWheelsGround)
            {
                carRigidbody.velocity = Vector3.Lerp(carRigidbody.velocity, Vector3.zero, accel * 0.1f);
                carRigidbody.AddForce(transform.forward * accel * currentTorque, ForceMode.Acceleration);

            }
        }
        else
        {
            wheelColliders[0].brakeTorque = wheelColliders[1].brakeTorque = wheelColliders[2].brakeTorque = wheelColliders[3].brakeTorque = 0f;
            wheelColliders[0].motorTorque = wheelColliders[1].motorTorque = wheelColliders[2].motorTorque = wheelColliders[3].motorTorque = accel;
            if (IsWheelsGround)
                carRigidbody.AddForce(transform.forward * accel * currentTorque, ForceMode.Acceleration);
        }

        //if moving forward, stop first by lerp then apply backward force
        if (CurrentSpeed > 0.1f && Vector3.Angle(transform.forward, carRigidbody.velocity) < 50f)
        {
            if (IsWheelsGround)
            {
                carRigidbody.velocity = Vector3.Lerp(carRigidbody.velocity, Vector3.zero, footbrake * 0.05f);
                carRigidbody.AddForce(-transform.forward * reverseTorque * footbrake, ForceMode.Acceleration);

            }
        }
        else if (footbrake > 0)
        {
            wheelColliders[0].brakeTorque = wheelColliders[1].brakeTorque = wheelColliders[2].brakeTorque = wheelColliders[3].brakeTorque = 0f;
            wheelColliders[0].motorTorque = wheelColliders[1].motorTorque = wheelColliders[2].motorTorque = wheelColliders[3].motorTorque = footbrake;
            if (IsWheelsGround)
                carRigidbody.AddForce(-transform.forward * reverseTorque * footbrake, ForceMode.Acceleration);
        }

    }

    /// <summary>
    ///  add a helper for steering when fast speed, add  more steering angle.
    /// </summary>
    private void SteerHelper()
    {
        if (!IsWheelsGround)
            return;
        if (Mathf.Abs(oldRotation - transform.eulerAngles.y) < 10f)
        {
            float turnadjust = (transform.eulerAngles.y - oldRotation) * steerHelper;
            Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
            carRigidbody.velocity = velRotation * carRigidbody.velocity;
        }
        oldRotation = transform.eulerAngles.y;

    }


    // this is used to add more grip in relation to speed
    private void AddDownForce()
    {
        if(IsWheelsGround)
            wheelColliders[0].attachedRigidbody.AddForce(-transform.up*downforce*
                                                        wheelColliders[0].attachedRigidbody.velocity.magnitude);
    }
}


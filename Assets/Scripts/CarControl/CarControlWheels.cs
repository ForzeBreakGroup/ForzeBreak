using System;
using UnityEngine;


public class CarControlWheels : NetworkPlayerMovement
{
    [SerializeField] private GameObject[] wheelMeshes = new GameObject[4];
    [SerializeField] private GameObject[] wheelCollidersObjects = new GameObject[4];
    [SerializeField] private Vector3 centreOfMassOffset;
    [SerializeField] private float maximumSteerAngle =30.0f;
    [Range(0, 1)] [SerializeField] private float steerHelper; // 0 is raw physics , 1 the car will grip in the direction it is facing
    [SerializeField] private float fullTorqueOverAllWheels =2500f;
    [SerializeField] private float reverseTorque=1500f;
    [SerializeField] private float downforce = 100f;
    [SerializeField] private float topspeed = 50;


    private WheelCollider[] wheelColliders = new WheelCollider[4];
    private WheelEffects[] wheelEffects = new WheelEffects[4];
    private Quaternion[] wheelMeshLocalRotations;
    private Vector3 prevpos, pos;
    private float steerAngle;
    private int gearNum;
    private float gearFactor;
    private float oldRotation;
    private float currentTorque;
    private Rigidbody carRigidbody;
    private const float reversingThreshold = 0.01f;
    
    public bool IsBoosting { get; set; }
    public bool IsWheelsGround { get; set; }
    public bool IsAnyWheelGround { get; set; }
    public bool Skidding { get; private set; }
    public float BrakeInput { get; private set; }
    public float CurrentSteerAngle{ get { return steerAngle; }}
    public float CurrentSpeed{ get { return carRigidbody.velocity.magnitude*2.23693629f; }}
    public float MaxSpeed { get; set; }
    public float Revs { get; private set; }
    public float AccelInput { get; private set; }

    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < 4; i++)
        {
            wheelColliders[i] = wheelCollidersObjects[i].GetComponent<WheelCollider>();
            wheelEffects[i] = wheelCollidersObjects[i].GetComponent<WheelEffects>();
        }

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

    
    
    


    public void Move(float steering, float accel, float footbrake, float handbrake)
    {

        IsWheelsGround = true;
        IsAnyWheelGround = false;
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
        handbrake = Mathf.Clamp(handbrake, 0, 1);

        Debug.Log("acc:" + accel + "   footbrake:" + footbrake);



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


    private void CapSpeed()
    {
        if (!IsBoosting && MaxSpeed > topspeed + 0.1f)
            MaxSpeed = Mathf.Lerp(MaxSpeed, topspeed, 0.1f);



        float speed = carRigidbody.velocity.magnitude;
        speed *= 2.23693629f;
        if(Vector3.Angle(transform.forward, carRigidbody.velocity) > 150f)
        {
            if (speed > MaxSpeed/4)
                carRigidbody.velocity = (MaxSpeed/(4*2.23693629f)) * carRigidbody.velocity.normalized;
        }
        else
        {
            if (speed > MaxSpeed)
                carRigidbody.velocity = (MaxSpeed / 2.23693629f) * carRigidbody.velocity.normalized;
        }

    }


    private void ApplyDrive(float accel, float footbrake)
    {

        if (CurrentSpeed > 1f && Vector3.Angle(transform.forward, carRigidbody.velocity) > 170f)
        {
            if (IsWheelsGround)
                carRigidbody.velocity = Vector3.Lerp(carRigidbody.velocity, Vector3.zero, accel * 0.001f);
        }
        else
        {
            wheelColliders[0].brakeTorque = wheelColliders[1].brakeTorque = wheelColliders[2].brakeTorque = wheelColliders[3].brakeTorque = 0f;
            wheelColliders[0].motorTorque = wheelColliders[1].motorTorque = wheelColliders[2].motorTorque = wheelColliders[3].motorTorque = accel;
            if (IsWheelsGround)
                carRigidbody.AddForce(transform.forward * accel * currentTorque, ForceMode.Acceleration);
        }


        if (CurrentSpeed > 1f && Vector3.Angle(transform.forward, carRigidbody.velocity) < 50f)
        {
            if (IsWheelsGround)
                carRigidbody.velocity = Vector3.Lerp(carRigidbody.velocity, Vector3.zero, footbrake * 0.05f);
        }
        else if (footbrake > 0)
        {
            wheelColliders[0].brakeTorque = wheelColliders[1].brakeTorque = wheelColliders[2].brakeTorque = wheelColliders[3].brakeTorque = 0f;
            wheelColliders[0].motorTorque = wheelColliders[1].motorTorque = wheelColliders[2].motorTorque = wheelColliders[3].motorTorque = footbrake;
            if (IsWheelsGround)
                carRigidbody.AddForce(-transform.forward * reverseTorque * footbrake, ForceMode.Acceleration);
        }

    }


    private void SteerHelper()
    {
        if (!IsWheelsGround)
            return;
        // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
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

  
    // checks if the wheels are spinning and is so does three things
    // 1) emits particles
    // 2) plays tiure skidding sounds
    // 3) leaves skidmarks on the ground
    // these effects are controlled through the WheelEffects class
    //private void CheckForWheelSpin()
    //{
    //    // loop through all wheels
    //    for (int i = 0; i < 4; i++)
    //    {
    //        WheelHit wheelHit;
    //        wheelColliders[i].GetGroundHit(out wheelHit);

    //        // is the tire slipping above the given threshhold
    //        if (Mathf.Abs(wheelHit.forwardSlip) >= slipLimit || Mathf.Abs(wheelHit.sidewaysSlip) >= slipLimit)
    //        {
    //            wheelEffects[i].EmitTyreSmoke();
                
    //            continue;
    //        }
            
    //        // end the trail generation
    //        wheelEffects[i].EndSkidTrail();
    //    }
    //}

}


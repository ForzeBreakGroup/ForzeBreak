using System;
using UnityEngine;

internal enum CarDriveType
{
    FrontWheelDrive,
    RearWheelDrive,
    FourWheelDrive
}

internal enum SpeedType
{
    MPH,
    KPH
}

public class CarControlWheels : NetworkPlayerMovement
{
    [SerializeField] private CarDriveType carDriveType = CarDriveType.FourWheelDrive;
    [SerializeField] private GameObject[] wheelMeshes = new GameObject[4];
    [SerializeField] private GameObject[] wheelCollidersObjects = new GameObject[4];
    [SerializeField] private Vector3 centreOfMassOffset;
    [SerializeField] private float maximumSteerAngle =30.0f;
    [Range(0, 1)] [SerializeField] private float steerHelper; // 0 is raw physics , 1 the car will grip in the direction it is facing
    [Range(0, 1)] [SerializeField] private float tractionControl; // 0 is no traction control, 1 is full interference
    [SerializeField] private float fullTorqueOverAllWheels =2500f;
    [SerializeField] private float reverseTorque=1500f;
    [SerializeField] private float maxHandbrakeTorque=1e+8f;
    [SerializeField] private float downforce = 100f;
    [SerializeField] private SpeedType speedType;
    [SerializeField] private float topspeed = 50;
    [SerializeField] private static int numOfGears = 5;
    [SerializeField] private float revRangeBoundary = 1f;
    [SerializeField] private float slipLimit = 0.2f;
    [SerializeField] private float brakeTorque = 1e+8f;


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
    public bool Skidding { get; private set; }
    public float BrakeInput { get; private set; }
    public float CurrentSteerAngle{ get { return steerAngle; }}
    public float CurrentSpeed{ get { return carRigidbody.velocity.magnitude*2.23693629f; }}
    public float MaxSpeed { get; set; }
    public float Revs { get; private set; }
    public float AccelInput { get; private set; }

    // Use this for initialization
    private void Start()
    {
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

        maxHandbrakeTorque = float.MaxValue;

        carRigidbody = GetComponent<Rigidbody>();
        currentTorque = fullTorqueOverAllWheels - (tractionControl * fullTorqueOverAllWheels);

        MaxSpeed = topspeed;
    }

    public void Move(float steering, float accel, float footbrake, float handbrake)
    {
        IsWheelsGround = true;
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
        }

        //clamp input values
        steering = Mathf.Clamp(steering, -1, 1);
        AccelInput = accel = Mathf.Clamp(accel, 0, 1);
        BrakeInput = footbrake = -1 * Mathf.Clamp(footbrake, -1, 0);
        handbrake = Mathf.Clamp(handbrake, 0, 1);

        if (handbrake <= 0f)
        {
            wheelColliders[2].brakeTorque = 0;
            wheelColliders[3].brakeTorque = 0;
        }
        //Set the steer on the front wheels.
        //Assuming that wheels 0 and 1 are the front wheels.
        steerAngle = steering * maximumSteerAngle;
        wheelColliders[0].steerAngle = steerAngle;
        wheelColliders[1].steerAngle = steerAngle;

        SteerHelper();
        ApplyDrive(accel, footbrake);
        CapSpeed();

        //Set the handbrake.
        //Assuming that wheels 2 and 3 are the rear wheels.
        if (handbrake > 0f)
        {
            var hbTorque = handbrake * maxHandbrakeTorque;
            wheelColliders[2].brakeTorque = hbTorque;
            wheelColliders[3].brakeTorque = hbTorque;
        }


        CalculateRevs();
        GearChanging();

        AddDownForce();
        //CheckForWheelSpin();
        TractionControl();
    }

    private void GearChanging()
    {
        float f = Mathf.Abs(CurrentSpeed/MaxSpeed);
        float upgearlimit = (1/(float) numOfGears)*(gearNum + 1);
        float downgearlimit = (1/(float) numOfGears)*gearNum;

        if (gearNum > 0 && f < downgearlimit)
        {
            gearNum--;
        }

        if (f > upgearlimit && (gearNum < (numOfGears - 1)))
        {
            gearNum++;
        }
    }


    // simple function to add a curved bias towards 1 for a value in the 0-1 range
    private static float CurveFactor(float factor)
    {
        return 1 - (1 - factor)*(1 - factor);
    }


    // unclamped version of Lerp, to allow value to exceed the from-to range
    private static float ULerp(float from, float to, float value)
    {
        return (1.0f - value)*from + value*to;
    }


    private void CalculateGearFactor()
    {
        float f = (1/(float) numOfGears);
        // gear factor is a normalised representation of the current speed within the current gear's range of speeds.
        // We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
        var targetGearFactor = Mathf.InverseLerp(f*gearNum, f*(gearNum + 1), Mathf.Abs(CurrentSpeed/MaxSpeed));
        gearFactor = Mathf.Lerp(gearFactor, targetGearFactor, Time.deltaTime*5f);
    }


    private void CalculateRevs()
    {
        // calculate engine revs (for display / sound)
        // (this is done in retrospect - revs are not used in force/power calculations)
        CalculateGearFactor();
        var gearNumFactor = gearNum/(float) numOfGears;
        var revsRangeMin = ULerp(0f, revRangeBoundary, CurveFactor(gearNumFactor));
        var revsRangeMax = ULerp(revRangeBoundary, 1f, gearNumFactor);
        Revs = ULerp(revsRangeMin, revsRangeMax, gearFactor);
    }




    private void CapSpeed()
    {
        if (!IsBoosting && MaxSpeed > topspeed + 0.1f)
            MaxSpeed = Mathf.Lerp(MaxSpeed, topspeed, 0.5f);



        float speed = carRigidbody.velocity.magnitude;
        switch (speedType)
        {
            case SpeedType.MPH:

                speed *= 2.23693629f;
                if (speed > MaxSpeed)
                    carRigidbody.velocity = (MaxSpeed / 2.23693629f) * carRigidbody.velocity.normalized;
                break;

            case SpeedType.KPH:
                speed *= 3.6f;
                if (speed > MaxSpeed)
                    carRigidbody.velocity = (MaxSpeed / 3.6f) * carRigidbody.velocity.normalized;
                break;
        }
    }


    private void ApplyDrive(float accel, float footbrake)
    {
        if (CurrentSpeed > 0.1 && Vector3.Angle(transform.forward, carRigidbody.velocity) > 170f)
            wheelColliders[0].brakeTorque = wheelColliders[1].brakeTorque = wheelColliders[2].brakeTorque = wheelColliders[3].brakeTorque = brakeTorque* accel;
        else
        {
            float thrustTorque;
            switch (carDriveType)
            {
                case CarDriveType.FourWheelDrive:
                    thrustTorque = accel * (currentTorque / 4f);

                    wheelColliders[0].brakeTorque = wheelColliders[1].brakeTorque = wheelColliders[2].brakeTorque = wheelColliders[3].brakeTorque = 0f;
                    wheelColliders[0].motorTorque = wheelColliders[1].motorTorque = wheelColliders[2].motorTorque = wheelColliders[3].motorTorque = thrustTorque;
                    break;

                case CarDriveType.FrontWheelDrive:
                    thrustTorque = accel * (currentTorque / 2f);
                    wheelColliders[0].brakeTorque = wheelColliders[1].brakeTorque = wheelColliders[2].brakeTorque = wheelColliders[3].brakeTorque = 0f;
                    wheelColliders[0].motorTorque = wheelColliders[1].motorTorque = thrustTorque;
                    break;

                case CarDriveType.RearWheelDrive:
                    thrustTorque = accel * (currentTorque / 2f);
                    wheelColliders[0].brakeTorque = wheelColliders[1].brakeTorque = wheelColliders[2].brakeTorque = wheelColliders[3].brakeTorque = 0f;
                    wheelColliders[2].motorTorque = wheelColliders[3].motorTorque = thrustTorque;
                    break;
            }
        }

        
        if (CurrentSpeed > 0.1 && Vector3.Angle(transform.forward, carRigidbody.velocity) < 50f)
        {
            wheelColliders[0].brakeTorque = wheelColliders[1].brakeTorque = wheelColliders[2].brakeTorque = wheelColliders[3].brakeTorque = brakeTorque * footbrake;
        }
        else if (footbrake > 0)
        {
            wheelColliders[0].brakeTorque = wheelColliders[1].brakeTorque = wheelColliders[2].brakeTorque = wheelColliders[3].brakeTorque = 0f;
            wheelColliders[0].motorTorque = wheelColliders[1].motorTorque = wheelColliders[2].motorTorque = wheelColliders[3].motorTorque = -reverseTorque/4f * footbrake;
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

        

    // crude traction control that reduces the power to wheel if the car is wheel spinning too much
    private void TractionControl()
    {
        WheelHit wheelHit;
        switch (carDriveType)
        {
            case CarDriveType.FourWheelDrive:
                // loop through all wheels
                for (int i = 0; i < 4; i++)
                {
                    wheelColliders[i].GetGroundHit(out wheelHit);

                    AdjustTorque(wheelHit.forwardSlip);
                }
                break;

            case CarDriveType.RearWheelDrive:
                wheelColliders[2].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);

                wheelColliders[3].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);
                break;

            case CarDriveType.FrontWheelDrive:
                wheelColliders[0].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);

                wheelColliders[1].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);
                break;
        }
    }


    private void AdjustTorque(float forwardSlip)
    {
        if (forwardSlip >= slipLimit && currentTorque >= 0)
        {
            currentTorque -= 10 * tractionControl;
        }
        else
        {
            currentTorque += 10 * tractionControl;
            if (currentTorque > fullTorqueOverAllWheels)
            {
                currentTorque = fullTorqueOverAllWheels;
            }
        }
    }
    // checks if the wheels are spinning and is so does three things
    // 1) emits particles
    // 2) plays tiure skidding sounds
    // 3) leaves skidmarks on the ground
    // these effects are controlled through the WheelEffects class
    private void CheckForWheelSpin()
    {
        // loop through all wheels
        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelHit;
            wheelColliders[i].GetGroundHit(out wheelHit);

            // is the tire slipping above the given threshhold
            if (Mathf.Abs(wheelHit.forwardSlip) >= slipLimit || Mathf.Abs(wheelHit.sidewaysSlip) >= slipLimit)
            {
                wheelEffects[i].EmitTyreSmoke();
                
                continue;
            }
            
            // end the trail generation
            wheelEffects[i].EndSkidTrail();
        }
    }

}


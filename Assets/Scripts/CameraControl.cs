using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Robin Zhou
 * 
 * Description:
 * Camera Control, handle the location and rotation of camera.
 * Should bind to a parent object of the camera instead of the camera itself.
 */
public class CameraControl : MonoBehaviour {

    /// <summary>
    /// Target to follow.
    /// </summary>
    public GameObject target;


    
    /// <summary>
    /// Speed of moving when following target
    /// </summary>
    [SerializeField] private float moveSpeed = 15f;
    /// <summary>
    /// speed of rotation in Y Axis(Up Axis) when following target's rotation.
    /// </summary>
    [SerializeField] private float turnSpeed = 15f;
    /// <summary>
    /// When target spin angle is greater than spinTurnLimist, camera will stop following rotation, often use for drifting.
    /// </summary>
    [SerializeField] private float spinTurnLimit = 0f;
    /// <summary>
    /// Will camera follows target's rotation in X Axis (Left Axis).
    /// </summary>
    [SerializeField] private bool followRoll = false;
    /// <summary>
    /// Will camera follows target's rotaiton in Z Axis (Forward Axis).
    /// </summary>
    [SerializeField] private bool followTilt = false;
    /// <summary>
    /// Spin angle from last frame.
    /// </summary>
    private float lastFlatAngle;
    /// <summary>
    /// A factor that damping camera's rotation when drifting
    /// </summary>
    private float currentTurnAmount;
    /// <summary>
    /// Used by Mathf.SmoothDamp().
    /// </summary>
    private float turnSpeedVelocityChange;


    //private CarUserControl carUserControl;
    //private float currentRotation;
    //private float oldCameraHorizontal;

    private Vector3 rollUp = Vector3.up;

    void FixedUpdate () {
        FollowTarget(Time.fixedDeltaTime);
	}
    
    /// <summary>
    /// Follow function, call it every frame.
    /// </summary>
    /// <param name="deltaTime">deltaTime between every call</param>
    private void FollowTarget(float deltaTime)
    {
       if(!(deltaTime>0) || target == null)
        {
            return;
        }

       //if(carUserControl == null)
       // {
       //     carUserControl = target.GetComponent<CarUserControl>();
       // }
       //float cameraHorizontal = Input.GetAxis("Camera_Horizontal_Controller" + carUserControl.playerNum);
       //float cameraHorizontalMouse = Input.GetAxis("Camera_Horizontal_Mouse");
       //cameraHorizontal = (cameraHorizontal == 0) ? cameraHorizontalMouse : cameraHorizontal;

       


        Vector3 targetForward = target.transform.forward;
        Vector3 targetUp = target.transform.up;

        float currentFlatAngle = Mathf.Atan2(targetForward.x, targetForward.z) * Mathf.Rad2Deg;
        //damping rotation when target's turn angle is greater than spinTurnLimit
        if (spinTurnLimit > 0)
        {
            float targetSpinSpeed = Mathf.Abs(Mathf.DeltaAngle(lastFlatAngle, currentFlatAngle)) / deltaTime;
            float desiredTurnAmount = Mathf.InverseLerp(spinTurnLimit, spinTurnLimit * 0.75f, targetSpinSpeed);
            float turnReactSpeed = (currentTurnAmount > desiredTurnAmount ? 0.1f : 1f);

            currentTurnAmount = Mathf.SmoothDamp(currentTurnAmount, desiredTurnAmount, ref turnSpeedVelocityChange, turnReactSpeed);
        }
        else
        {
            currentTurnAmount = 1;
        }
        lastFlatAngle = currentFlatAngle;
        
        //In case camera switch too fast when forward vector is too small
        if (targetForward.sqrMagnitude < float.Epsilon)
            targetForward = transform.forward;

        if (!followRoll)
            targetForward.y = 0;

        if (followTilt)
            rollUp = Vector3.Slerp(rollUp, targetUp, 3 * deltaTime);

        Quaternion rotation = Quaternion.LookRotation(targetForward, rollUp);

        //change rotation and position
        transform.position = Vector3.Lerp(transform.position, target.transform.position, deltaTime * moveSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, turnSpeed * currentTurnAmount * deltaTime);


        //if (Math.Abs(cameraHorizontal) > 0.0001f)
        //{
        //    currentRotation += 0.5f * cameraHorizontal;
        //    rotation = Quaternion.Euler(0, currentRotation, 0);
        //    transform.rotation = Quaternion.Lerp(transform.rotation, rotation, turnSpeed * currentTurnAmount * deltaTime);
        //}else
        //{
        //    currentRotation = 0f;
        //}
    }
}

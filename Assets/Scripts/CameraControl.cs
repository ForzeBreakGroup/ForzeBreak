using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Robin
 * 
 * Description:
 * Camera Control, handle the location and rotation of camera.
 * 
 */
internal enum FollowType
{
    FollowVelocity,
    FollowTilt,
}
public class CameraControl : MonoBehaviour {

    
    [SerializeField] private FollowType followType = FollowType.FollowTilt;
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float turnSpeed = 15f;
    [SerializeField] private float rollSpeed = 3f;
    [SerializeField] private float targetVelocityLowerLimit = 0f;
    [SerializeField] private float smoothTurnTime = 0.2f;
    [SerializeField] private float spinTurnLimit = 180;

    private GameObject target;
    private float lastFlatAngle;
    private Rigidbody targetRigidbody;
    private float currentTurnAmount;
    private float turnSpeedVelocityChange;
    private Vector3 rollUp = Vector3.up;
    
    void Awake()
    {
        target = transform.parent.gameObject;
        targetRigidbody = target.GetComponent<Rigidbody>();
    }
	
    
	void FixedUpdate () {
        FollowTarget(Time.deltaTime);
	}

    private void FollowTarget(float deltaTime)
    {
       if(!(deltaTime>0) || target == null)
        {
            return;
        }

        Vector3 targetForward = target.transform.forward;
        Vector3 targetUp = target.transform.up;

        switch(followType)
        {
            case FollowType.FollowTilt:
                float currentFlatAngle = Mathf.Atan2(targetForward.x, targetForward.z) * Mathf.Rad2Deg;
                if(spinTurnLimit > 0)
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

                break;
            case FollowType.FollowVelocity:
                if (targetRigidbody.velocity.magnitude > targetVelocityLowerLimit)
                {
                    targetForward = targetRigidbody.velocity.normalized;
                    targetUp = Vector3.up;
                }
                else
                {
                    targetUp = Vector3.up;
                }
                
                currentTurnAmount = Mathf.SmoothDamp(currentTurnAmount, 1, ref turnSpeedVelocityChange, smoothTurnTime);

                targetForward.y = 0;
                if(targetForward.sqrMagnitude < float.Epsilon)
                {
                    targetForward = transform.forward;
                }
                break;
        }



        transform.position = Vector3.Lerp(transform.position, target.transform.position, deltaTime * moveSpeed);
        Quaternion rollRotation = Quaternion.LookRotation(targetForward, rollUp);

        rollUp = rollSpeed > 0 ? Vector3.Slerp(rollUp, targetUp, rollSpeed * deltaTime) : Vector3.up;
        transform.rotation = Quaternion.Lerp(transform.rotation, rollRotation, turnSpeed * currentTurnAmount * deltaTime);

    }
}

﻿using System;
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
public class CameraControl : MonoBehaviour {

    
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float turnSpeed = 15f;
    [SerializeField] private float smoothTurnTime = 0.2f;
    [SerializeField] private float spinTurnLimit = 180;
    [SerializeField] private bool followRoll = false;
    [SerializeField] private bool followTilt = false;

    private GameObject target;
    private float lastFlatAngle;
    private Rigidbody targetRigidbody;
    private float currentTurnAmount;
    private float turnSpeedVelocityChange;
    private Vector3 rollUp = Vector3.up;

    private Vector3 oldPosition;
    void Awake()
    {
        target = transform.root.gameObject;
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
        
        transform.position = Vector3.Lerp(oldPosition, target.transform.position, deltaTime * moveSpeed);
        oldPosition = transform.position;
        
        if (targetForward.sqrMagnitude < float.Epsilon)
            targetForward = transform.forward;

        if (!followRoll)
            targetForward.y = 0;
        if (followTilt)
            rollUp = Vector3.Slerp(rollUp, targetUp, 3 * deltaTime);

        Quaternion rollRotation = Quaternion.LookRotation(targetForward, rollUp);

        transform.rotation = Quaternion.Lerp(transform.rotation, rollRotation, turnSpeed * currentTurnAmount * deltaTime);

    }
}

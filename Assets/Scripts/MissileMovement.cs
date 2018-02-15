using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class MissileMovement : Photon.MonoBehaviour
{
    public GameObject target;

    [Range(1, 2)]
    [SerializeField] private float flyupDuration = 1.0f;

    [Range(1, 10)]
    [SerializeField] private float diveSpeed = 1.0f;

    private float elapsedTime = 0.0f;

    private void FixedUpdate()
    {
        if (target != null)
        {
            elapsedTime += Time.deltaTime;

            // Missile will fly up towards sky first
            if (elapsedTime < flyupDuration)
            {

            }

            // Then it will dive towards the target with accelerated speed
            else
            {

            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Create explosion at impact point
    }
}

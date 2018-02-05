using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Only for hanlding collision between players, and keeping record of damages of the player vehicle.
 * Analyze the collision and applies different force based on collision analysis
 */
public class DamageSystem : NetworkPlayerCollision
{
    /// <summary>
    /// Sealed class to define constants for damage system
    /// </summary>
    sealed class DamageSystemConstants
    {
        public const float baseDamagePercentage = 100.0f;
        public const float maxDamagePercentage = 500.0f;
    }

    #region Private Members
    /// <summary>
    /// Damage Amplification % that will be applied into calculation of damage, bound by min and max value determined in constant class
    /// </summary>
    [Range(DamageSystemConstants.baseDamagePercentage, DamageSystemConstants.maxDamagePercentage)]
    [SerializeField] private float damageAmplifyPercentage = DamageSystemConstants.baseDamagePercentage;

    /// <summary>
    /// Allowance angle to determine the vehicle is collider or receiver
    /// </summary>
    [Range(0, 45)]
    [SerializeField] private float colliderAngle = 45.0f;

    /// <summary>
    /// Additional upward effect applied to Collider
    /// </summary>
    [Range(0, 1)]
    [SerializeField] private float colliderUpwardEffect = 0.0f;

    /// <summary>
    /// Additional amplification force to Receiver
    /// </summary>
    [Range(0, 3)]
    [SerializeField] private float receiverAdditionalAmplification = 1.0f;

    /// <summary>
    /// Explosion radius applied to the Receiver, should be propotional to Receiver Upward Effect
    /// </summary>
    [Range(1, 10)]
    [SerializeField] private float receiverExplosionRadius = 3.0f;

    /// <summary>
    /// Additional upward effect applied to Receiver
    /// </summary>
    [Range(1, 10)]
    [SerializeField] private float receiverUpwardEffect = 2.0f;

    /// <summary>
    /// Enables logging information on Console
    /// </summary>
    [SerializeField] private bool enableLog = false;
    #endregion

    #region Private Methods
    protected override CollisionResult CollisionEvent(Collision collision)
    {
        return CollisionResult.Receiver;
    }

    protected override void ResolveCollision(CollisionResult collisionResult, float force)
    {
        if (collisionResult == CollisionResult.Collider)
        {

        }
        else
        {

        }
    }

    /// <summary>
    /// Use the angle between normalized contact point and normalized vehicle rotation to determine if the
    /// vehicle is receiving the collision or the cause of collision
    /// </summary>
    /// <param name="contactNorm">Average of collision contact points norm</param>
    /// <returns>Result of collision analysis, receiver or collider</returns>
    private CollisionResult AnalyzeCollision(Vector3 contactNorm)
    {
        float collisionAngle = Vector3.Angle(GetComponent<Rigidbody>().velocity, -contactNorm);

        if (enableLog)
        {
            Debug.Log("Vehicle Velocity: " + GetComponent<Rigidbody>().velocity + ", Contact Point: " + contactNorm + ", Angle: " + collisionAngle);
        }

        if (collisionAngle < colliderAngle)
        {
            return CollisionResult.Collider;
        }

        // Otherwise, the entity is receiver
        return CollisionResult.Receiver;
    }

    /// <summary>
    /// Applying amplified forces to the Collider using reciprocal force received from collision impulse
    /// </summary>
    /// <param name="impulse">Impulse force from Collision class</param>
    /// <param name="collisionPoint">Impact point from Collision class</param>
    private void ColliderReciprocalForce(Vector3 impulse, Vector3 collisionPoint)
    {
        // Force = |impact force| * norm(P) * % dmg
        Vector3 amplifiedReciprocalForce = impulse.magnitude * collisionPoint.normalized * damageAmplifyPercentage / 100.0f;

        // Logging information on console
        if (enableLog)
        {
            Debug.Log("Collider Reciprocal Force: ");
            Debug.Log("Applying " + amplifiedReciprocalForce + " At Location: " + (collisionPoint + new Vector3(0, -colliderUpwardEffect, 0)));
            Debug.Log("Amplified from: " + impulse + " to: " + amplifiedReciprocalForce);
        }

        // Apply reciprocal force at collision point with upward effect
        GetComponent<Rigidbody>().AddForceAtPosition(amplifiedReciprocalForce, collisionPoint + new Vector3(0, -colliderUpwardEffect, 0), ForceMode.Impulse);
    }

    /// <summary>
    /// Applying amplified explosion force to the Receiver using impulse received from collision
    /// </summary>
    /// <param name="impulse">Impulse force from Collision class</param>
    /// <param name="collisionPoint">Impact point from Collision class</param>
    private void ReceiverAmplifiedForce(Vector3 impulse, Vector3 collisionPoint)
    {
        // Force = |impact force| * additional amplification * % dmg
        //float explosionForce = impulse.magnitude * receiverAdditionalAmplification * damageAmplifyPercentage / 100.0f;

        // Logging information on console
        if (enableLog)
        {
            Debug.Log("Receiver Amplified Force: ");
           // Debug.Log("Applying " + explosionForce + " at: " + collisionPoint + new Vector3(0, -receiverUpwardEffect, 0));
           // Debug.Log("Amplified from: " + impulse.magnitude + " to: " + explosionForce);
        }

        // Apply explosion force at specified location with upward effect
        GetComponent<Rigidbody>().AddForce((impulse + Vector3.up * receiverUpwardEffect) * damageAmplifyPercentage * receiverAdditionalAmplification * 10, ForceMode.Impulse);

        Debug.Log((impulse + Vector3.up * receiverUpwardEffect) * damageAmplifyPercentage * receiverAdditionalAmplification * 10);
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Increase the damaga percentage by the value given
    /// </summary>
    /// <param name="value"></param>
    public void IncreaseDamage(float value)
    {
        damageAmplifyPercentage += value;
    }

    /// <summary>
    /// Decrease the damage percentage by the value given, can not be lower than the default value
    /// </summary>
    /// <param name="value"></param>
    public void DecreaseDamage(float value)
    {
        damageAmplifyPercentage -= value;
        if (damageAmplifyPercentage < DamageSystemConstants.baseDamagePercentage)
        {
            damageAmplifyPercentage = DamageSystemConstants.baseDamagePercentage;
        }
    }
    #endregion
}

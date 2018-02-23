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

public enum CollisionEffect
{
    UpwardEffect,
    FlyOffEffect
};

public enum DamageThreshold
{
    Healthy,
    LightDamage,
    Damaged,
    HeavilyDamage,
};

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
    [SerializeField] private CollisionEffect effectMode = CollisionEffect.UpwardEffect;

    /// <summary>
    /// Damage Amplification % that will be applied into calculation of damage, bound by min and max value determined in constant class
    /// </summary>
    [Range(DamageSystemConstants.baseDamagePercentage, DamageSystemConstants.maxDamagePercentage)]
    [SerializeField]
    public float damageAmplifyPercentage = DamageSystemConstants.baseDamagePercentage;

    /// <summary>
    /// Allowance angle to determine the vehicle is collider or receiver
    /// </summary>
    [Range(0, 45)]
    [SerializeField]
    private float colliderAngle = 45.0f;

    /// <summary>
    /// Additional upward effect applied to Collider
    /// </summary>
    [Range(0, 1)]
    [SerializeField]
    private float colliderUpwardEffect = 0.0f;

    /// <summary>
    /// Additional amplification force to Receiver
    /// </summary>
    [Range(0, 3)]
    [SerializeField]
    private float receiverAdditionalAmplification = 1.0f;

    /// <summary>
    /// Explosion radius applied to the Receiver, should be propotional to Receiver Upward Effect
    /// </summary>
    [Range(1, 10)]
    [SerializeField]
    private float receiverExplosionRadius = 3.0f;

    /// <summary>
    /// Additional upward effect applied to Receiver
    /// </summary>
    [Range(1, 10)]
    [SerializeField]
    private float receiverUpwardEffect = 2.0f;

    /// <summary>
    /// Enables logging information on Console
    /// </summary>
    [SerializeField] private bool enableLog = false;

    private Rigidbody rb;
    #endregion

    #region Private Methods

    /// <summary>
    /// Using this life-hook method to initialize
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected override PlayerCollisionResult CollisionEvent(Collision collision, out float force, out Vector3 contactPoint)
    {
        PlayerCollisionResult result = AnalyzeCollision(collision);

        force = collision.impulse.magnitude;
        contactPoint = collision.contacts[0].point;

        if (photonView.isMine)
        {
            if (result == PlayerCollisionResult.Collider)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            else
            {
                ApplyExplosionForce(force, contactPoint, 300.0f);
            }
        }

        return result;
    }

    protected override void ResolveCollision(PlayerCollisionResult collisionResult, float force, Vector3 contactPoint)
    {
        if (enableLog)
        {
            Debug.Log("Resolving Collision Event: " + photonView.viewID);
            Debug.Log(string.Format("Collision Result: {0}, Receiving Force: {1}, Contact Point: {2}", collisionResult, force, contactPoint));
            Debug.Log(string.Format("Vehicle Position: {0}, Distance Between Contact Point: {1}", rb.position, Vector3.Distance(rb.position, contactPoint)));
        }

        if (collisionResult == PlayerCollisionResult.Collider)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            ApplyExplosionForce(force, contactPoint, 300.0f);
        }
    }

    /// <summary>
    /// Use the angle between normalized contact point and normalized vehicle rotation to determine if the
    /// vehicle is receiving the collision or the cause of collision
    /// </summary>
    /// <param name="contactNorm">Average of collision contact points norm</param>
    /// <returns>Result of collision analysis, receiver or collider</returns>
    private PlayerCollisionResult AnalyzeCollision(Collision collision)
    {
        PlayerCollisionResult result = PlayerCollisionResult.Receiver;
        Vector3 contactNormal = collision.contacts[0].normal;
        float selfCollisionAngle = Vector3.Angle(rb.velocity, -contactNormal);
        float otherCollisionAngle = Vector3.Angle(collision.rigidbody.velocity, -contactNormal);

        /* Compares two collider's velocity, bigger one with correct angle wins the clash
         * If both velocity are equal, the one with better angle wins the clash
         * If both velocity and angle are equal, both flies away
         */
        // Compare velocity and hitting angle
        if (rb.velocity.magnitude >= collision.rigidbody.velocity.magnitude)
        {
            result = PlayerCollisionResult.Collider;

            // Handling case of same velocity
            if (rb.velocity.magnitude == collision.rigidbody.velocity.magnitude)
            {
                // Wins the clash if collision angle is better
                if (selfCollisionAngle > otherCollisionAngle)
                {
                    result = PlayerCollisionResult.Collider;
                }
                else
                {
                    result = PlayerCollisionResult.Receiver;
                }
            }
        }

        if (enableLog)
        {
            Debug.Log("Analyze Collision: " + result);
            Debug.Log(string.Format("My force: {0}, Their force: {1}", rb.velocity.magnitude, collision.rigidbody.velocity.magnitude));
            Debug.Log(string.Format("Contact Point: {0}, Contact Point Normal: {1}", collision.contacts[0].point, collision.contacts[0].normal));
            Debug.Log(string.Format("My velocity: {0}, Angle: {1}", rb.velocity, selfCollisionAngle));
            Debug.Log(string.Format("Other's velocity: {0}, Angle: {1}", collision.rigidbody.velocity, otherCollisionAngle));
        }

        // Otherwise, the entity is receiver
        return result;
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
    private void ApplyExplosionForce(float impulse, Vector3 collisionPoint, float radius)
    {
        Debug.Log(impulse);
        switch (effectMode)
        {
            case CollisionEffect.FlyOffEffect:
                rb.AddExplosionForce(impulse * damageAmplifyPercentage / 100.0f, collisionPoint, radius, 0.2f, ForceMode.Impulse);
                break;
            case CollisionEffect.UpwardEffect:
            default:
                rb.AddExplosionForce(impulse * damageAmplifyPercentage / 100.0f, collisionPoint, radius, 3.0f, ForceMode.Impulse);
                break;
        }
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

    [PunRPC]
    public void CreateExplosion(float force, Vector3 explosionCenter, float radius)
    {
        IncreaseDamage(30);
        ApplyExplosionForce(force, explosionCenter, radius);
    }
    #endregion
}

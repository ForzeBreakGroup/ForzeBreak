using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Jason Lin
 * 
 * Description: 
 * Player object damage system.
 * Handles OnCollision event calls and amplifies the impulse forces based on current damage percentages
 */
public enum CollisionType
{
    /// <summary>
    /// DEFAULT Collision Type - Amplifies Y axis non-angular forces by damage percentage with magnitude of original impulse force
    /// </summary>
    DEFAULT,

    /// <summary>
    /// EXPLOSION Collision Type - Amplifies Y axis non-angular forces similar to DEFAULT scheme, but also applies fraction of it as angular force
    /// </summary>
    EXPLOSION,

    /// <summary>
    /// SLIDING Collision Type - Amplifies X and Z axis non-angular forces to create a sliding effect
    /// </summary>
    SLIDING
}


[RequireComponent(typeof(Collision))]
public class DamageSystem : MonoBehaviour
{
    #region Public Members
    /// <summary>
    /// Collision Type effects defined by enum
    /// </summary>
    public CollisionType collisionType = CollisionType.DEFAULT;

    /// <summary>
    /// Damage amplification percentage, by default is 1.0f = 100%
    /// </summary>
    public float damagePercentage = 1.0f;
    #endregion

    #region Private Members
    [SerializeField] private bool enableLog = false;
    [SerializeField] private float internalAmplify = 1.0f;
    [SerializeField] private float explosionRadius = 3.0f;
    [SerializeField] private float upwardEffect = 2.0f;
    #endregion

    #region Private Methods
    /// <summary>
    /// Using this life-hook method to initialize
    /// </summary>
    private void Awake()
    {
        damagePercentage = 1.0f;
    }

    /// <summary>
    /// Life-hook method when collision happens
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        
        // Amplifies the impulse forces applied to this object by damage percentages if colliding object is another Player
        if (collision.transform.tag == "Player")
        {
            switch (collisionType)
            {
                case CollisionType.DEFAULT:
                    // Amplify collision force based on damage percentage
                    AmplifyNonAngularYForce(collision.impulse, collision.contacts[0].point);
                    break;
                case CollisionType.EXPLOSION:
                    ExplosionAmplify(collision.impulse, collision.contacts[0].point);
                    break;
                case CollisionType.SLIDING:
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Amplifies Y axis's non-angular force using the magnitude of impact force and multiplies with damagae percentage
    /// </summary>
    /// <param name="impulse"> Original impulse force gathered from collision </param>
    private void AmplifyNonAngularYForce(Vector3 impulse, Vector3 collisionPoint)
    {
        Vector3 amplifiedForce = (new Vector3(0, impulse.magnitude, 0) + impulse) * damagePercentage;

        if (enableLog)
        {
            Debug.Log("Amplify Non-Angular Y Force");
            Debug.Log("Location: " + collisionPoint);
            Debug.Log("Before Amplify: " + impulse);
            Debug.Log("After Amplify: " + amplifiedForce);
        }

        GetComponent<Rigidbody>().AddForceAtPosition(amplifiedForce, collisionPoint, ForceMode.Impulse);
    }

    private void ExplosionAmplify(Vector3 impulse, Vector3 collisionPoint)
    {
        float explosionForce = impulse.magnitude * internalAmplify * damagePercentage;

        if (enableLog)
        {
            Debug.Log("Explosion Force");
            Debug.Log("Location: " + collisionPoint);
            Debug.Log("Explosion Force: " + explosionForce);
        }

        GetComponent<Rigidbody>().AddExplosionForce(explosionForce, collisionPoint, explosionRadius, upwardEffect, ForceMode.Impulse);
    }

    #endregion

    #region Public Methods
    /// <summary>
    /// Increase the damaga percentage by the value given
    /// </summary>
    /// <param name="value"></param>
    public void IncreaseDamage(float value)
    {
        damagePercentage += value;
    }

    /// <summary>
    /// Decrease the damage percentage by the value given, can not be lower than the default value
    /// </summary>
    /// <param name="value"></param>
    public void DecreaseDamage(float value)
    {
        damagePercentage -= value;
        if (damagePercentage < 1.0f)
        {
            damagePercentage = 1.0f;
        }
    }
    #endregion
}

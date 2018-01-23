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
[RequireComponent(typeof(Collision))]
public class DamageSystem : MonoBehaviour
{
    #region Public Members
    /// <summary>
    /// Damage amplification percentage, by default is 1.0f = 100%
    /// </summary>
    public float damagePercentage;
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
            // Amplify collision force based on damage percentage
            this.GetComponent<Rigidbody>().AddForceAtPosition(AmplifyForce(collision.impulse), collision.contacts[0].point, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// Apply additional upforward force to fly the colliding object
    /// </summary>
    /// <param name="impulse"> Original impulse force gathered from collision </param>
    /// <returns></returns>
    private Vector3 AmplifyForce(Vector3 impulse)
    {
        // Use the impulse force magnitude as additional upward force
        Vector3 newImpulse = (impulse + new Vector3(0, impulse.magnitude, 0)) * damagePercentage;
        Debug.Log("After: " + newImpulse);

        return newImpulse;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collision))]
public class DamageSystem : MonoBehaviour
{
    #region Private Members
    public float damagePercentage;
    #endregion

    #region Private Methods
    private void Awake()
    {
        damagePercentage = 1.0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Apply explosive force to other player
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
        Debug.Log("Before: " + impulse);
        // Get the magnitude of X, Z coordinate force
        Vector3 newImpulse = (impulse + new Vector3(0, impulse.magnitude, 0)) * damagePercentage;
        Debug.Log("After: " + newImpulse);

        return newImpulse;
    }
    #endregion

    #region Public Methods
    public void IncreaseDamage(float value)
    {
        damagePercentage += value;
    }

    public void DecreaseDamage(float value)
    {
        damagePercentage -= value;
    }
    #endregion
}

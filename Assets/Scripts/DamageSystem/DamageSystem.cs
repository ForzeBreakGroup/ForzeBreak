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
    #region Private Members
    /// <summary>
    /// Enables logging information on Console
    /// </summary>
    [SerializeField] private bool enableLog = false;

    /// <summary>
    /// Internal reference to player object's rigidbody
    /// </summary>
    private Rigidbody rb;

    #endregion

    #region Private Methods

    /// <summary>
    /// Using this life-hook method to initialize
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Using given force and colliding point to calculate the force needed to simulate flyoff trajectory
    /// Then apply the force to vehicle
    /// </summary>
    /// <param name="impulse"></param>
    /// <param name="collisionPoint"></param>
    private void TrajectoryCollision(float impulse, Vector3 collisionPoint)
    {
        // Calculate the flyoff distance based on received force and damage amplification
        // Amplified flyoff distance = amplify %  * damage received
        float amplifiedFlyoffDistance = impulse;

        // Calculate the normalized flying direction, and change Y axis to align with upward effect
        Vector3 normalizedPoint = (transform.root.position - collisionPoint).normalized;
        normalizedPoint.y = 0.45f;

        // Using trajectory formula d = v^2/g sin 2 theta to predict the velocity needed to hit the desire location
        // Assuming the launching leviation is on flat surface, and theta is 45 degree, this will remove sin 2theta to 1
        // Then the velocity can be found by v^2 = d * g, where d is the distance to travel, g is the gravitational force set by Unity project
        Vector3 velocity = Mathf.Sqrt(amplifiedFlyoffDistance * Physics.gravity.magnitude) * normalizedPoint;
        GetComponent<Rigidbody>().AddForce(velocity, ForceMode.VelocityChange);

        if (enableLog)
        {
            Debug.Log(string.Format("Fly Off Distance: {0}, Calculated velocity: {1}", amplifiedFlyoffDistance, velocity));
        }
    }

    #endregion

    #region Public Methods
    /// <summary>
    /// Apply the damage force to the vehicle, this method will automatically increase the damage percentage based on the distance from the impact center and radius of the effective area
    /// </summary>
    /// <param name="force"></param>
    /// <param name="explosionCenter"></param>
    /// <param name="radius"></param>
    public void ApplyDamageForce(float force, Vector3 explosionCenter, float radius, int ownerId)
    {
        // Only apply damage force if the vehicle is controlled by self
        if (gameObject.GetPhotonView().isMine)
        {
            lastReceivedDamageFrom = ownerId;

            if (force > 0)
            {
                // Calculates damage received based on the force and explosion radius
                float damage = force * (radius - Mathf.Abs(Vector3.Distance(transform.root.position, explosionCenter))) / radius;
                damage = Mathf.Clamp(damage, 0, Mathf.Infinity);

                TrajectoryCollision(damage, explosionCenter);

                PlayCollisionEffect(this.transform.position);
            }
        }
    }
    #endregion
}

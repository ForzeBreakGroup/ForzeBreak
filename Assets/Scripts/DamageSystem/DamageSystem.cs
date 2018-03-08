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
 
public enum DamageThreshold
{
    Healthy,
    LightlyDamaged,
    HeavilyDamaged,
    Critical
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
    /// <summary>
    /// Damage Amplification % that will be applied into calculation of damage, bound by min and max value determined in constant class
    /// </summary>
    [Range(DamageSystemConstants.baseDamagePercentage, DamageSystemConstants.maxDamagePercentage)]
    [SerializeField]
    private float damageAmplifyPercentage = DamageSystemConstants.baseDamagePercentage;
    
    [SerializeField]
    private float healthyThreshold = 150.0f;

    [SerializeField]
    private float lightlyDamageThreshold = 250.0f;

    [SerializeField]
    private float heavilyDamageThreshold = 375.0f;

    /// <summary>
    /// Enables logging information on Console
    /// </summary>
    [SerializeField] private bool enableLog = false;

    /// <summary>
    /// Upward effect, higher the number, higher it flies
    /// </summary>
    [Range(0, 1)]
    [SerializeField] private float upwardEffect = 0.45f;

    /// <summary>
    /// Internal reference to player object's rigidbody
    /// </summary>
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
        float amplifiedFlyoffDistance = damageAmplifyPercentage / 100.0f * impulse;

        // Calculate the normalized flying direction, and change Y axis to align with upward effect
        Vector3 normalizedPoint = (transform.root.position - collisionPoint).normalized;
        normalizedPoint.y = upwardEffect;

        // Using trajectory formula d = v^2/g sin 2 theta to predict the velocity needed to hit the desire location
        // Assuming the launching leviation is on flat surface, and theta is 45 degree, this will remove sin 2theta to 1
        // Then the velocity can be found by v^2 = d * g, where d is the distance to travel, g is the gravitational force set by Unity project
        Vector3 velocity = Mathf.Sqrt(amplifiedFlyoffDistance * Physics.gravity.magnitude) * normalizedPoint;
        GetComponent<Rigidbody>().AddForce(velocity, ForceMode.VelocityChange);

        Debug.Log("Camerashake");

        if (enableLog)
        {
            Debug.Log(string.Format("Fly Off Distance: {0}, Calculated velocity: {1}", amplifiedFlyoffDistance, velocity));
        }
    }

    public override void SerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.SerializeView(stream, info);

        if (stream.isWriting)
        {
            stream.SendNext(damageAmplifyPercentage);
        }
        else if(stream.isReading)
        {
            damageAmplifyPercentage = (float)stream.ReceiveNext();
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

    public void CreateExplosion(float force, Vector3 explosionCenter, float radius)
    {
        if (gameObject.GetPhotonView().isMine)
        {
            Debug.Log(string.Format("Force: {0}, Center: {1}, Radius: {2}", force, explosionCenter, radius));

            // Calculates damage received based on the force and explosion radius
            float damage = force * (radius - Mathf.Abs(Vector3.Distance(transform.root.position, explosionCenter))) / radius;
            damage = Mathf.Clamp(damage, 0, Mathf.Infinity);

            IncreaseDamage(damage);
            TrajectoryCollision(damage, explosionCenter);
        }
    }

    public DamageThreshold GetDamageThreshold()
    {
        // Healthy
        if(damageAmplifyPercentage < healthyThreshold)
        {
            return DamageThreshold.Healthy;
        }

        // Slightly damaged
        else if (damageAmplifyPercentage < lightlyDamageThreshold)
        {
            return DamageThreshold.LightlyDamaged;
        }

        // Heavyly damaged
        else if (damageAmplifyPercentage < heavilyDamageThreshold)
        {
            return DamageThreshold.HeavilyDamaged;
        }

        // Critical
        else
        {
            return DamageThreshold.Critical;
        }
    }
    #endregion
}

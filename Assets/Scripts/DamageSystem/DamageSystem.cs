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
 
/// <summary>
/// Enum defines the damage threshold
/// </summary>
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
    /// Public class to define constants for damage system
    /// </summary>
    public class DamageSystemConstants
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
    public float damageAmplifyPercentage = DamageSystemConstants.baseDamagePercentage;
    
    /// <summary>
    /// Adjustable threshold value for a healthy vehicle condition, if damages percentage is under this number, it is considered healthy
    /// </summary>
    [SerializeField]
    private float healthyThreshold = 150.0f;

    /// <summary>
    /// Adjustable threshold value for a lightly damaged vehicle condition, if damage percentage is under this number, it is considered lightly damaged
    /// </summary>
    [SerializeField]
    private float lightlyDamageThreshold = 250.0f;

    /// <summary>
    /// Adjustable threshold value for a heavily damaged vehicle condition
    /// </summary>
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

    /// <summary>
    /// Internal boolean flag to enable the damage amplification based on damage sustained
    /// </summary>
    [SerializeField]
    private bool enableAmplifyDamage = false;

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
        float amplifiedFlyoffDistance = impulse;
        if (enableAmplifyDamage)
        {
            amplifiedFlyoffDistance *= damageAmplifyPercentage / 100.0f;
        }

        // Calculate the normalized flying direction, and change Y axis to align with upward effect
        Vector3 normalizedPoint = (transform.root.position - collisionPoint).normalized;
        normalizedPoint.y = upwardEffect;

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
            // Calculates damage received based on the force and explosion radius
            float damage = force * (radius - Mathf.Abs(Vector3.Distance(transform.root.position, explosionCenter))) / radius;
            damage = Mathf.Clamp(damage, 0, Mathf.Infinity);

            IncreaseDamage(damage);
            TrajectoryCollision(damage, explosionCenter);

            PlayCollisionEffect(this.transform.position);
        }
    }

    /// <summary>
    /// Returns damage threshold based on internal damage percentage
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Serialize the damage percentage to all remote clients
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="info"></param>
    public override void SerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.SerializeView(stream, info);

        if (stream.isWriting)
        {
            stream.SendNext(damageAmplifyPercentage);
        }
        else if (stream.isReading)
        {
            damageAmplifyPercentage = (float)stream.ReceiveNext();
        }
    }
    #endregion
}

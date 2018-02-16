using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public enum FlyUpEffect
{
    StraightUp,
    HalfWay,
};

public class MissileMovement : NetworkPowerUpMovement
{
    public GameObject target;

    [Range(1, 2)]
    [SerializeField] private float flyupDuration = 1.0f;
    private Vector3 flyUpDestination;

    [SerializeField] private FlyUpEffect flyUpEffectOption = FlyUpEffect.HalfWay;

    [Range(1, 10)]
    [SerializeField] private float diveSpeed = 1.0f;

    private float elapsedTime = 0.0f;
    private enum MissileMovementState
    {
        FlyUp,
        Diving
    };
    private MissileMovementState state = MissileMovementState.FlyUp;

    protected override void Awake()
    {
        base.Awake();
        GetComponent<Collider>().enabled = false;
    }

    public void Fire()
    {
        // Calculate the flyUpDestination
        if (flyUpEffectOption == FlyUpEffect.HalfWay)
        {
            flyUpDestination = new Vector3( (target.transform.position.x + transform.position.x) / 2,
                                            (target.transform.position.y + transform.position.y) / 2,
                                            (target.transform.position.z + transform.position.z) / 2);
            flyUpDestination += new Vector3(0, 12, 0);
        }
        else if (flyUpEffectOption == FlyUpEffect.StraightUp)
        {
            flyUpDestination = transform.position + new Vector3(0, 12, 0);
        }

        this.transform.LookAt(flyUpDestination);
    }

    protected override void Move()
    {
        base.Move();
        // Missile will fly up towards sky first
        switch (state)
        {
            case MissileMovementState.Diving:
                transform.LookAt(target.transform);
                rb.AddRelativeForce(Vector3.forward * 50, ForceMode.Force);
                break;
            case MissileMovementState.FlyUp:
            default:
                if (Vector3.Distance(transform.position, flyUpDestination) >= 1f)
                {
                    rb.AddRelativeForce(Vector3.forward * 20, ForceMode.Force);
                }
                else
                {
                    state = MissileMovementState.Diving;
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    GetComponent<Collider>().enabled = true;
                }
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 impactPos = transform.position;

        PhotonNetwork.Instantiate("Explosion1", impactPos, Quaternion.identity, 0);
        PhotonNetwork.Destroy(gameObject);
    }
}

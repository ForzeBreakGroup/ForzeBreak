using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class MissileMovement : NetworkPowerUpMovement
{
    public GameObject target;

    [Range(1, 2)]
    [SerializeField] private float flyupDuration = 1.0f;
    private Vector3 flyUpDestination;

    [Range(1, 10)]
    [SerializeField] private float diveSpeed = 10.0f;

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
        transform.LookAt(target.transform);
        flyUpDestination = transform.position + transform.forward;
        state = MissileMovementState.FlyUp;
    }

    protected override void Move()
    {
        base.Move();
        // Missile will fly up towards sky first
        switch (state)
        {
            case MissileMovementState.Diving:
                Quaternion q = Quaternion.LookRotation(target.transform.position - transform.position);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, q, diveSpeed * Time.deltaTime);
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
        if (collision.transform.root.tag != "PowerUp")
        {
            Vector3 impactPos = transform.position;

            PhotonNetwork.Instantiate("Explosion1", impactPos, Quaternion.identity, 0);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}

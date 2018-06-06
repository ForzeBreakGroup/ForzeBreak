using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Robin Zhou
 * 
 * Description:
 * Hook moving class.
 * 
 */
public class HookMovement : PowerUpMovement
{
    /// <summary>
    /// Bullet moving speed
    /// </summary>
    public float Velocity = 100f;
    /// <summary>
    /// Bullet existing time duration
    /// </summary>
    public float ExistingTime = 3f;

    public float ExistingTimeHooked = 15f;

    public float forceOnRope = 20000f;

    public GameObject source;
    public GameObject target;

    private Transform trail;
    private LineRenderer line;
    private float spawnTime = 0f;
    private Vector3 offset = new Vector3(0, 1.308f, 1.808f);
    Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Velocity * transform.forward;
        spawnTime = Time.time;
        trail = transform.Find("Trail");
        line = trail.GetComponent<LineRenderer>();


    }

    private void Update()
    {
        line.SetPosition(0, trail.position);

        if (source!=null)
        {
            line.SetPosition(1, source.transform.position);
        }
        else
        {
            line.SetPosition(1, trail.position);
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject go in gameObjects)
            {
                if (go.GetComponent<PhotonView>().ownerId == PowerUpData.OwnerID)
                {
                    source = go;
                    return;
                }
            }
        }

        if(target!=null)
        {
            if(transform.localPosition!= Vector3.zero)
                transform.localPosition = Vector3.zero;

        }

        if(source!=null && target!= null)
        {
            if (source.GetComponent<PhotonView>().isMine)
            {
                source.GetComponent<Rigidbody>().AddForce((target.transform.position - source.transform.position)* forceOnRope);
            }

            if (target.GetComponent<PhotonView>().isMine)
            {
                target.GetComponent<Rigidbody>().AddForce((source.transform.position - target.transform.position)* forceOnRope);
            }
        }


        if (Time.time > spawnTime + ExistingTime)
        {
            if (photonView.isMine)
            {
                DestroyPowerUpProjectile();
            }
        }
    }


    public void SetHookTarget(int targetID)
    {
        PhotonView.RPC("RpcSetHookTarget", PhotonTargets.All, targetID);
    }

    [PunRPC]
    public void RpcSetHookTarget(int targetID)
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in gameObjects)
        {
            if (go.GetComponent<PhotonView>().ownerId == targetID)
            {
                PhotonView.TransferOwnership(targetID);

                ExistingTime += ExistingTimeHooked;

                Rigidbody r = GetComponent<Rigidbody>();
                r.isKinematic = true;
                r.velocity = Vector3.zero;

                transform.SetParent(go.transform);
                target = go;
                return;
            }
        }
    }

}

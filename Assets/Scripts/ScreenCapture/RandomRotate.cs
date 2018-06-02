using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotate : MonoBehaviour {

    public float rotateSpeed = 2f;

    public float threshold = 0.2f;

    private Vector3 dir = Vector3.zero;
    private float timer = 0f;
    private float nextNewRotation = 5f;

    private void Awake()
    {
        dir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * rotateSpeed;
    }
    void Update ()
    {
        

        if (Time.time>timer)
        {
            timer = Time.time + nextNewRotation;

            dir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * rotateSpeed;
            if ((Quaternion.Euler(dir) * transform.up).y < threshold)
                dir.x = -dir.x;

            if ((Quaternion.Euler(dir) * transform.up).y < threshold)
                dir.z = -dir.z;

        }

        if (transform.up.y < threshold)
        {
            transform.up = new Vector3(transform.up.x, threshold, transform.up.z);
            transform.GetChild(0).LookAt(transform.position);
            dir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * rotateSpeed;
            if ((Quaternion.Euler(dir) * transform.up).y < threshold)
                dir.x = -dir.x;

            if ((Quaternion.Euler(dir) * transform.up).y < threshold)
                dir.z = -dir.z;
        }


        transform.GetChild(0).LookAt(transform.position);
        transform.Rotate(dir);
    }
}

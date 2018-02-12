using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject target;
    private float distance = 5;

    private void LateUpdate()
    {
        transform.position = new Vector3(target.transform.position.x, target.transform.position.y + distance/2, target.transform.position.z - distance);
        this.transform.LookAt(target.transform);
    }
}

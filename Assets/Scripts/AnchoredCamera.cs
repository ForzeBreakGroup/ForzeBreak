using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchoredCamera : MonoBehaviour
{
    private void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * Quaternion.Euler(Vector3.up), 0.1f);
    }
}

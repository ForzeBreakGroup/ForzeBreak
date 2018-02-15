using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleSystem : MonoBehaviour
{
    private bool reticleEnable = false;
    private Camera cam;
    private Collider objectCollider;

    private GameObject target;

    public void EnableReticleSystem(GameObject target)
    {
        if (!target)
        {
            Debug.LogError("Target Cannot be Null");
        }
        this.target = target;
        cam = NetworkManager.playerCamera;
        objectCollider = target.transform.Find("Collider").GetComponentInChildren<Collider>();
        reticleEnable = true;
    }

    private void FixedUpdate()
    {
        if (reticleEnable)
        {
            GetComponent<Renderer>().enabled = true;
            // Test the target object's collider bound overlaps with player's camera
            if (GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(cam), objectCollider.bounds))
            {
                // Send a ray cast to ensure the object is not blocked
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit))
                {
                    // Check if raycast hit result matches the target
                    if (hit.transform.gameObject == target)
                    {
                        GetComponent<Renderer>().enabled = false;
                    }
                }
            }
        }
    }
}

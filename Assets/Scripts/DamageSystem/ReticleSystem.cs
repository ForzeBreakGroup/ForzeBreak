using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Arrow 3D tracker will switch to Reticle view if the target is within the camera.
 */
public class ReticleSystem : MonoBehaviour
{
    #region Public Members
    /// <summary>
    /// Boolean indicate if the target is in sight
    /// </summary>
    public bool targetInSight = false;

    /// <summary>
    /// The Target this arrow is pointing at
    /// </summary>
    public GameObject target;
    #endregion

    #region Private Members
    /// <summary>
    /// Boolean flag indicate if the ReticleSystem for this arrow is enabled
    /// </summary>
    private bool reticleEnable = false;

    /// <summary>
    /// Camera reference
    /// </summary>
    private Camera cam;

    /// <summary>
    /// The collider to obtain the bound from
    /// </summary>
    private Collider objectCollider;

    #endregion

    #region Private Methods

    /// <summary>
    /// Every physics update, this script calcualtes if target's collision bound is within camera's viewport
    /// If it is, it sends a raycast to make sure there's nothing in between
    /// Then switches to reticle mode when all conditions are satisfied
    /// </summary>
    private void FixedUpdate()
    {
        if (reticleEnable)
        {
            GetComponent<Renderer>().enabled = true;
            targetInSight = false;
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
                        targetInSight = true;
                    }
                }
            }
        }
    }

    #endregion

    #region Public Methods
    /// <summary>
    /// Explicit call to enable the reticle system
    /// </summary>
    /// <param name="target"></param>
    public void EnableReticleSystem(GameObject target)
    {
        if (!target)
        {
            Debug.LogError("Target Cannot be Null");
        }
        this.target = target;
        cam = NetworkManager.playerCamera;
        objectCollider = target.transform.Find("Colliders").GetComponentInChildren<Collider>();
        reticleEnable = true;
    }

    #endregion
}

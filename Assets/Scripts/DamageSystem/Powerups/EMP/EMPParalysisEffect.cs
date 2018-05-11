using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPParalysisEffect : MonoBehaviour
{
    private PSMeshRendererUpdater meshRendererUpdater;
    private GameObject playerInEffect;

    private void Awake()
    {
        if (transform.root.tag == "Player")
        {
            // Find the main mesh of the vehicle
            playerInEffect = transform.root.gameObject;
            Transform mainBody = transform.root.Find("Model").Find("Body").Find("Main");

            // Apply the paralysis effect on the material
            meshRendererUpdater = GetComponent<PSMeshRendererUpdater>();
            meshRendererUpdater.MeshObject = mainBody.gameObject;
            meshRendererUpdater.UpdateMeshEffect();
        }
    }

    public void DestroyAfterDuration(float duration)
    {
        StartCoroutine(DestroyAfterDelayTimer(duration));

        // Disable the player control during the duration
        if (playerInEffect != null)
        {
            playerInEffect.GetComponent<CarUserControl>().DisableCarControl(duration);
        }
    }

    IEnumerator DestroyAfterDelayTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(this.gameObject);
    }
}

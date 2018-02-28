using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ReticleUIState
{
    TARGET,
    MISS
}

public class ReticleAnimation : MonoBehaviour
{
    private Animator anim;
    private Image img;
    private RectTransform rect;
    private Vector3 defaultPos;
    
    private void Awake()
    {
        anim = GetComponent<Animator>();
        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        defaultPos = rect.localPosition;
        img.enabled = false;
    }

    public void DisplayReticleUI(ReticleUIState uiState, Vector3 targetLocation)
    {
        if (uiState == ReticleUIState.MISS)
        {
            ReticleMissUI();
        }
        else if (uiState == ReticleUIState.TARGET)
        {
            ReticleTargetUI(targetLocation);
        }
    }

    private void ReticleMissUI()
    {
        Debug.Log("ReticleMiss");
        // Move the UI to default position
        rect.localPosition = defaultPos;

        // Display the image and change the color to red
        img.enabled = true;
        img.color = Color.red;

        // Play miss animation
        anim.SetTrigger("TargetMiss");
    }

    private void ReticleTargetUI(Vector3 targetLocation)
    {
        // Move the reticle image to target position
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        rect.localPosition = targetLocation - screenCenter;

        // Display the image and change the color to green
        img.enabled = true;
        img.color = Color.green;

        // Play target found animation
        anim.SetTrigger("TargetHit");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Controls the reticle animation playback for target found and target not found
 */

/// <summary>
/// Enum defines Reticle UI state
/// </summary>
public enum ReticleUIState
{
    /// <summary>
    /// UI State defines the target is within view
    /// </summary>
    TARGET,

    /// <summary>
    /// UI State defines the target is not found
    /// </summary>
    MISS
}

public class ReticleAnimation : MonoBehaviour
{
    #region Private Members
    /// <summary>
    /// Animator reference
    /// </summary>
    private Animator anim;

    /// <summary>
    /// UI Image reference in Canvas for switching UI image
    /// </summary>
    private Image img;

    /// <summary>
    /// The RectTransform reference for moving position
    /// </summary>
    private RectTransform rect;

    /// <summary>
    /// Internal stored default position of original RectTransform
    /// </summary>
    private Vector3 defaultPos;
    #endregion

    #region Private Methods
    /// <summary>
    /// For initializing the internal references
    /// </summary>
    private void Awake()
    {
        // Find the internal references
        anim = GetComponent<Animator>();
        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        defaultPos = rect.localPosition;

        // Disable the image
        img.enabled = false;
    }
    
    /// <summary>
    /// Displays the target not found UI image at the default position, then play the corresponding animation
    /// </summary>
    private void ReticleMissUI()
    {
        // Move the UI to default position
        rect.localPosition = defaultPos;

        // Display the image and change the color to red
        img.enabled = true;
        img.color = Color.red;

        // Play miss animation
        anim.SetTrigger("TargetMiss");
    }

    /// <summary>
    /// Displays the target found UI image at the target location projected to screen pixel position, then play corresponding animation
    /// </summary>
    /// <param name="targetLocation"></param>
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
    #endregion

    #region Public Methods

    /// <summary>
    /// Public interface API for controlling which UI to display and where the target is located
    /// </summary>
    /// <param name="uiState"></param>
    /// <param name="targetLocation"></param>
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

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehicleStatsUIControl : MonoBehaviour
{
    private Animator anim;
    private bool isAnimating = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void OnHoverEnter()
    {
        if (!isAnimating)
        {
            isAnimating = true;
            anim.SetTrigger("Show");
        }
    }

    public void OnHoverExit()
    {
        if (!isAnimating)
        {
            isAnimating = true;
            anim.SetTrigger("Hide");
        }
    }

    private void Update()
    {
        // Detect holding option in controller
    }

    public void OnAnimationCompleteEvent()
    {
        isAnimating = false;
    }
}

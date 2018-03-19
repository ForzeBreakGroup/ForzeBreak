using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControl : MonoBehaviour
{
    protected bool controlEnabled = false;

    protected void Update()
    {
        if (controlEnabled)
        {
            UpdateUIControl();
        }
    }

    protected virtual void UpdateUIControl()
    {
        // Empty for child class to inherit
    }

    public virtual void EnableUIControl()
    {
        controlEnabled = true;
    }
}

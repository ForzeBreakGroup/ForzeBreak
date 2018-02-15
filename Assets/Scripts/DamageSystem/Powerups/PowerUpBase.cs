using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBase : MonoBehaviour
{
    protected virtual void OnPress()
    {

    }

    protected virtual void OnHold()
    {

    }

    protected virtual void OnRelease()
    {

    }

    private void Update()
    {
        // Get input from player
        if (Input.GetButtonDown("Controller_Button_Y"))
        {
            OnPress();
        }
        else if (Input.GetButtonUp("Controller_Button_Y"))
        {
            OnRelease();
        }
        else if (Input.GetButton("Controller_Button_Y"))
        {
            OnHold();
        }
    }
}

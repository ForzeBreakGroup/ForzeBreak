using System;
using UnityEngine;

/*
 * Author: Robin
 * 
 * Description:
 * Apply Input to CarControlWheels script, handle all inputs including keyboard and controller
 */
public class CarUserControl : NetworkPlayerInput
{
    public Color color;
    public int playerNum;

    private CarControlWheels carControlWheels; // the car controller we want to use
    private BoostControl boostControl;
    private FlipControl flipControl;

    private bool boost = false;
    private bool flip = false;

    private void Awake()
    {
        // get the car controller
        carControlWheels = GetComponent<CarControlWheels>();
        boostControl = GetComponent<BoostControl>();
        flipControl = GetComponent<FlipControl>();
        playerNum = 0;

    }

    private void FixedUpdate()
    {
        // keyboard Input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float handbrake = Input.GetAxis("Handbrake");

        // controller Input
        float controllerX = Input.GetAxis("Controller" + playerNum + "_X_Axis");
        float controllerTrigger = Input.GetAxis("Controller" + playerNum + "_Trigger_Axis");

        //if keyboard input is none, apply controller input
        h = (h == 0) ? controllerX : h;
        v = (v == 0) ? -controllerTrigger : v;
        carControlWheels.Move(h, v, v, handbrake);




        boost = Input.GetButton("Mouse_Left") || Input.GetButton("Controller" + playerNum + "_Button_B");
        flip = Input.GetButtonDown("Mouse_Right") || Input.GetButtonDown("Controller" + playerNum + "_Button_A");
        if (boostControl!=null)
        {
            if (boost)
                boostControl.Boost();
            else
                boostControl.Recover();
        }

        if(flipControl!=null)
            flipControl.Flip(flip,h);
    
    }

    private void Update()
    {
            
    }

    public void ChangeColor(Color c)
    {
        color = c;
        Material mat = transform.Find("Model").transform.Find("Body").GetComponent<MeshRenderer>().material;
        // Change the color of player vehicle to assigned color
        if (mat.color != color)
        {
            mat.color = color;
        }
    }
}


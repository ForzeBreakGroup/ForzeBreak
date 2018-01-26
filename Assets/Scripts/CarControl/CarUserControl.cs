using System;
using UnityEngine;
using UnityEngine.Networking;

/*
 * Author: Robin
 * 
 * Description:
 * Apply Input to CarControlWheels script, handle all inputs including keyboard and controller
 */
public class CarUserControl : NetworkBehaviour
{
    public GameObject cam;
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
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;
        // keyboard Input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float handbrake = Input.GetAxis("Handbrake");
        // controller Input
        float controllerX = Input.GetAxis("Controller_X_Axis");
        float controllerTrigger = Input.GetAxis("Controller_Trigger_Axis");

        //if keyboard input is none, apply controller input
        h = (h == 0) ? controllerX : h;
        v = (v == 0) ? -controllerTrigger : v;
        carControlWheels.Move(h, v, v, handbrake);

        if(boost)
            boostControl.Boost();
        else
            boostControl.Recover();

        if(flip)
            flipControl.Flip();

    }

    private void Update()
    {
        boost = Input.GetButton("Mouse_Left") || Input.GetButton("Controller_Button_B");
        flip = Input.GetButtonDown("Mouse_Right") || Input.GetButtonDown("Controller_Button_A");
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("OnStartLocalPlayer");
        base.OnStartLocalPlayer();
        transform.Find("Model").transform.Find("Tank_Body").GetComponent<MeshRenderer>().material.color = Color.blue;
        GameObject obj = Instantiate<GameObject>(cam, this.transform);
    }
}


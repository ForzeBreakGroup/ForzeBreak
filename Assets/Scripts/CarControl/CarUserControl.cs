using System;
using UnityEngine;

/*
 * Author: Robin
 * 
 * Description:
 * Apply Input to CarControlWheels script, handle all inputs including keyboard and controller
 */
public class CarUserControl : MonoBehaviour
{
    public Color color;
    public GameObject cam;
    private CarControlWheels carControlWheels; // the car controller we want to use
    private BoostControl boostControl;
    private FlipControl flipControl;

    private bool boost = false;
    private bool flip = false;

    private void Start()
    {
        GameObject obj = Instantiate<GameObject>(cam, this.transform);
        // get the car controller
        carControlWheels = GetComponent<CarControlWheels>();
        boostControl = GetComponent<BoostControl>();
        flipControl = GetComponent<FlipControl>();

        // Change the color of player vehicle to assigned color
        Material mat = transform.Find("Model").transform.Find("Tank_Body").GetComponent<MeshRenderer>().material;
        if (mat.color != color)
        {
            mat.color = color;
        }
    }

    private void FixedUpdate()
    {
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
        
        flipControl.Flip(flip,h);

    }

    private void Update()
    {
        boost = Input.GetButton("Mouse_Left") || Input.GetButton("Controller_Button_B");
        flip = Input.GetButtonDown("Mouse_Right") || Input.GetButtonDown("Controller_Button_A");        
    }

    public void ChangeColor(Color c)
    {
        color = c;
    }
}


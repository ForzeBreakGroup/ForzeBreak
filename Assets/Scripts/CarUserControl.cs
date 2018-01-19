using System;
using UnityEngine;
using UnityEngine.Networking;

public class CarUserControl : NetworkBehaviour
{
    public GameObject camera;
    private CarControlWheels m_Car; // the car controller we want to use
    private CameraControl cameraControl; // Camera for the player


    private void Awake()
    {
        // get the car controller
        m_Car = GetComponent<CarControlWheels>();
       
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        // pass the input to the car!
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float handbrake = Input.GetAxis("Handbrake");

        float controllerX = Input.GetAxis("Controller_X_Axis");
        float controllerTrigger = Input.GetAxis("Controller_Trigger_Axis");

        h = (h == 0) ? controllerX : h;
        v = (v == 0) ? -controllerTrigger : v;
        m_Car.Move(h, v, v, handbrake);

    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("OnStartLocalPlayer");
        base.OnStartLocalPlayer();
        transform.Find("Model").transform.Find("Tank_Body").GetComponent<MeshRenderer>().material.color = Color.blue;
    }
}


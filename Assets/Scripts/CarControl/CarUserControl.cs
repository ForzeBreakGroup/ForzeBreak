using System;
using UnityEngine;
using System.Collections;

/*
 * Author: Robin
 * 
 * Description:
 * Apply Input to CarControlWheels script, handle all inputs including keyboard and controller
 */
public class CarUserControl : NetworkPlayerInput
{
    /// <summary>
    /// for local game, indicate the player
    /// </summary>
    public int controllerNum;

    private CarControlWheels carControlWheels;
    private BoostControl boostControl;
    private FlipControl flipControl;

    public bool boost = false;
    private bool flip = false;

    private bool isDisabled = false;


    private FMOD.Studio.EventInstance engine;

    protected override void Awake()
    {
        base.Awake();

        // get the car controller
        carControlWheels = GetComponent<CarControlWheels>();
        boostControl = GetComponent<BoostControl>();
        flipControl = GetComponent<FlipControl>();
        controllerNum = 0;


        engine = FMODUnity.RuntimeManager.CreateInstance("event:/SFX_Diegetic/SFX_VehicleEngine");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(engine, transform, GetComponent<Rigidbody>());
        //engine.start();
    }

    protected override void PlayerInputUpdate()
    {
        base.PlayerInputUpdate();

        if (!isDisabled)
        {
            boost = Input.GetButton("Boost_Mouse") || Input.GetButton("Boost_Controller" + controllerNum);
            flip = Input.GetButtonDown("Flip_Keyboard") || Input.GetButtonDown("Flip_Controller" + controllerNum);
        }
        else
        {
            boost = false;
            flip = false;
        }

    }

    protected override void PlayerInputFixedUpdate()
    {
        base.PlayerInputFixedUpdate();

        float h = 0f;
        float v = 0f;

        if (!isDisabled)
        {
            // keyboard Input
            h = Input.GetAxis("Horizontal_Keyboard");
            v = Input.GetAxis("Vertical_Keyboard");

            // controller Input
            float controllerX = Input.GetAxis("Horizontal_Controller" + controllerNum);
            float controllerTrigger = Input.GetAxis("Trigger_Axis_Controller" + controllerNum);

            //if keyboard input is none, apply controller input
            h = (h == 0) ? controllerX : h;
            v = (v == 0) ? -controllerTrigger : v;
        }


        carControlWheels.Move(h, v, v);

        //flip
        if (flipControl != null)
            flipControl.Flip(flip, h);

        if (boost)
            engine.setParameterValue("Speed", 1f);
        else
            engine.setParameterValue("Speed", GetComponent<Rigidbody>().velocity.magnitude / 20);
    }

    public override void SerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.SerializeView(stream, info);

        if (stream.isWriting)
        {
            stream.SendNext(boost);
        }
        else if (stream.isReading)
        {
            boost = (bool)stream.ReceiveNext();
        }
    }

    public void DisableCarControl(float duration)
    {
        StartCoroutine(EnableAfterDelay(duration));
    }

    IEnumerator EnableAfterDelay(float duration)
    {
        isDisabled = true;
        yield return new WaitForSeconds(duration);
        isDisabled = false;
    }
}


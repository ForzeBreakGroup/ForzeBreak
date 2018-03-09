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
    /// <summary>
    /// for local game, indicate the player
    /// </summary>
    public int playerNum;

    private CarControlWheels carControlWheels;
    private BoostControl boostControl;
    private FlipControl flipControl;

    public bool boost = false;
    public bool flip = false;

    /// <summary>
    /// engine sound
    /// </summary>
    private FMOD.Studio.EventInstance engine;


    protected override void Awake()
    {
        base.Awake();

        // get the car controller
        carControlWheels = GetComponent<CarControlWheels>();
        boostControl = GetComponent<BoostControl>();
        flipControl = GetComponent<FlipControl>();
        playerNum = 0;

        engine = FMODUnity.RuntimeManager.CreateInstance("event:/SFX_Diegetic/SFX_VehicleEngine");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(engine,transform,GetComponent<Rigidbody>());
        engine.start();
    }

    protected override void PlayerInputUpdate()
    {
        base.PlayerInputUpdate();

        // keyboard Input
        float h = Input.GetAxis("Horizontal_Keyboard");
        float v = Input.GetAxis("Vertical_Keyboard");

        // controller Input
        float controllerX = Input.GetAxis("Horizontal_Controller" + playerNum);
        float controllerTrigger = Input.GetAxis("Trigger_Axis_Controller" + playerNum);

        //if keyboard input is none, apply controller input
        h = (h == 0) ? controllerX : h;
        v = (v == 0) ? -controllerTrigger : v;
        carControlWheels.Move(h, v, v);

        boost = Input.GetButton("Boost_Mouse") || Input.GetButton("Boost_Controller" + playerNum);
        flip = Input.GetButtonDown("Flip_Keyboard") || Input.GetButtonDown("Flip_Controller" + playerNum);

        //flip
        if (flipControl != null)
            flipControl.Flip(flip, h);

        //change engine sound pitch
        engine.setParameterValue("Speed", GetComponent<Rigidbody>().velocity.magnitude / 20);
    }

    public override void SerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.SerializeView(stream, info);

        if (stream.isWriting)
        {
            stream.SendNext(boost);
            stream.SendNext(flip);
        }
        else if (stream.isReading)
        {
            boost = (bool)stream.ReceiveNext();
            flip = (bool)stream.ReceiveNext();
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Controls the engine sound based on rigidbody velocity
 */
public class VehicleSound : MonoBehaviour
{
    [FMODUnity.EventRef]
    [SerializeField]
    public string engineSoundref;


    [FMODUnity.EventRef]
    [SerializeField]
    public string flipSoundref;

    [FMODUnity.EventRef]
    [SerializeField]
    public string boostSoundref;
    /// <summary>
    /// engine sound
    /// </summary>
    private FMOD.Studio.EventInstance engine;

    public FMOD.Studio.EventInstance flipSound;
    public FMOD.Studio.EventInstance boostSound;

    // Use this for initialization
    void Awake ()
    {
        engine = FMODUnity.RuntimeManager.CreateInstance(engineSoundref);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(engine, transform, GetComponent<Rigidbody>());
        engine.start();

        flipSound = FMODUnity.RuntimeManager.CreateInstance(flipSoundref);
        boostSound = FMODUnity.RuntimeManager.CreateInstance(boostSoundref);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //change engine sound pitch
        engine.setParameterValue("Speed", GetComponent<Rigidbody>().velocity.magnitude / GetComponent<BoostControl>().boostMaxSpeed);
    }

    public void PlayFlipSound()
    {
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(flipSound, transform, GetComponent<Rigidbody>());
        flipSound.start();
    }

    public void PlayBoostSound()
    {
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(boostSound, transform, GetComponent<Rigidbody>());
        boostSound.start();
    }


    private void OnDestroy()
    {
        engine.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        engine.release();
        flipSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        flipSound.release();
        boostSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        boostSound.release();

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Controls the engine sound based on rigidbody velocity
 */
public class EngineSound : MonoBehaviour
{
    /// <summary>
    /// engine sound
    /// </summary>
    private FMOD.Studio.EventInstance engine;

    // Use this for initialization
    void Awake ()
    {
        engine = FMODUnity.RuntimeManager.CreateInstance("event:/SFX_Diegetic/SFX_VehicleEngine");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(engine, transform, GetComponent<Rigidbody>());
        engine.start();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //change engine sound pitch
        engine.setParameterValue("Speed", GetComponent<Rigidbody>().velocity.magnitude / 20);
    }
}

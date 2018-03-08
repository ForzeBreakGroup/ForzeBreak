using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Robin Zhou
 * 
 * Description:
 * Missle Sound.
 */
public class MissileSound : MonoBehaviour {

    private FMOD.Studio.EventInstance fireSound;
    void Start () {

        fireSound = FMODUnity.RuntimeManager.CreateInstance("event:/SFX_Diegetic/SFX_MissileLaunch");

        FMODUnity.RuntimeManager.AttachInstanceToGameObject(fireSound, transform, GetComponent<Rigidbody>());
        fireSound.start();

    }

}

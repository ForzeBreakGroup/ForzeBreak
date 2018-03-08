using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Robin Zhou
 * 
 * Description:
 * Bullet sound
 */
public class BulletSound : MonoBehaviour {

    private FMOD.Studio.EventInstance fireSound;
    void Awake () {

        fireSound = FMODUnity.RuntimeManager.CreateInstance("event:/SFX_Diegetic/SFX_Cannon");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(fireSound, transform, GetComponent<Rigidbody>());
        fireSound.start();

    }

}

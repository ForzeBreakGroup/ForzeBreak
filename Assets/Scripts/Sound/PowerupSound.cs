using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSound : MonoBehaviour {


    [System.Serializable]
    public struct SingleSound
    {
        [SerializeField]
        public string SoundName;

        [FMODUnity.EventRef]
        [SerializeField]
        public string Soundref;

        [SerializeField]
        public bool Attached;
    };


    [FMODUnity.EventRef]
    [SerializeField]
    public string launchSoundref;
    [SerializeField]
    public bool launchSoundAttached;

    [FMODUnity.EventRef]
    [SerializeField]
    public string followSoundref;
    [SerializeField]
    public bool followSoundAttached;

    [SerializeField]
    public SingleSound[] soundList;

    private FMOD.Studio.EventInstance followSound;

    protected virtual void Awake()
    {
        if (launchSoundref != "")
        {
            
            if (launchSoundAttached)
                FMODUnity.RuntimeManager.PlayOneShotAttached(launchSoundref, gameObject);
            else
                FMODUnity.RuntimeManager.PlayOneShot(launchSoundref,transform.position);
        }

        if (followSoundref != "")
        {
            followSound = FMODUnity.RuntimeManager.CreateInstance(followSoundref);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(followSound, transform, GetComponent<Rigidbody>());
            followSound.start();
           
        }
    }


    public void PlaySound(int index)
    {
        if (index<soundList.Length)
        {
            if (soundList[index].Soundref != "")
            {
                if (soundList[index].Attached)
                    FMODUnity.RuntimeManager.PlayOneShotAttached(soundList[index].Soundref, gameObject);
                else
                    FMODUnity.RuntimeManager.PlayOneShot(soundList[index].Soundref, gameObject.transform.position);
            }
        }
    }

    public void OnDestroy()
    {
        followSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        followSound.release();
    }


}

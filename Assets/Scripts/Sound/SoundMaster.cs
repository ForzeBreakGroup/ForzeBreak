using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMaster : MonoBehaviour {

    [FMODUnity.EventRef]
    [SerializeField]
    private string BGMRef;

    private FMOD.Studio.EventInstance BGM;

    public static SoundMaster instance = null;

    FMOD.Studio.Bus masterGroup;
    FMOD.Studio.Bus BGMGroup;
    FMOD.Studio.Bus diegeticGroup;
    FMOD.Studio.Bus nonDiegeticGroup;

    [Range(0, 1)] public float masterVolume = 0.5f;
    [Range(0, 1)] public float BGMVolume = 0.5f;
    [Range(0, 1)] public float diegeticVolume = 0.5f;
    [Range(0, 1)] public float nonDiegeticVolume = 0.5f;



    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        masterGroup = FMODUnity.RuntimeManager.GetBus("bus:/Master");
        BGMGroup = FMODUnity.RuntimeManager.GetBus("bus:/Master/BGM");
        diegeticGroup = FMODUnity.RuntimeManager.GetBus("bus:/Master/SFX_Diegetic");
        nonDiegeticGroup = FMODUnity.RuntimeManager.GetBus("bus:/Master/SFX_Non Diegetic");

        BGM = FMODUnity.RuntimeManager.CreateInstance(BGMRef);
        BGM.start();
    }

    private void Update()
    {
        masterGroup.setVolume(masterVolume);
        BGMGroup.setVolume(BGMVolume);
        diegeticGroup.setVolume(diegeticVolume);
        nonDiegeticGroup.setVolume(nonDiegeticVolume);


    }

    private void OnDestroy()
    {
        BGM.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        BGM.release();
    }

    public void ChangeBGMStage(int stage)
    {
        BGM.setParameterValue("Stage", stage);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingTextUIControl : MonoBehaviour
{
    Text txt;
    Slider slider;
    SoundMaster soundMaster;

    private enum AudioSettingsCategory
    {
        Master,
        BGM,
        Effects,
        UI
    }

    [SerializeField]
    private AudioSettingsCategory audioSettingCategory = AudioSettingsCategory.Master;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        txt = transform.GetComponentInChildren<Text>();
        soundMaster = FindObjectOfType<SoundMaster>();
    }

    private void Update()
    {
        txt.text = slider.value.ToString();
        ApplySoundVolume(slider.value);
    }

    private void ApplySoundVolume(float sliderValue)
    {
        switch (audioSettingCategory)
        {
            case AudioSettingsCategory.Master:
                soundMaster.masterVolume = sliderValue / 100.0f;
                break;
            case AudioSettingsCategory.BGM:
                soundMaster.BGMVolume = sliderValue / 100.0f;
                break;
            case AudioSettingsCategory.Effects:
                soundMaster.diegeticVolume = sliderValue / 100.0f;
                break;
            case AudioSettingsCategory.UI:
                soundMaster.nonDiegeticVolume = sliderValue / 100.0f;
                break;
        }
    }
}

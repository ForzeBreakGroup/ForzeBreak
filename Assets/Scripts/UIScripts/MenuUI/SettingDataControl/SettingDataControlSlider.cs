using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingDataControlSlider : SettingDataControlBase
{
    [SerializeField]
    private Slider slider;

    public override void Load()
    {
        if (PlayerPrefs.HasKey(settingDataKey))
        {
            slider.value = PlayerPrefs.GetFloat(settingDataKey);
        }
    }

    public override void Save()
    {
        PlayerPrefs.SetFloat(settingDataKey, slider.value);
    }
}

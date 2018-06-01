using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingDataControlToggle : SettingDataControlBase
{
    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    public override void Load()
    {
        toggle.isOn = (PlayerPrefs.GetInt(settingDataKey) > 0);
    }

    public override void Save()
    {
        PlayerPrefs.SetInt(settingDataKey, (toggle.isOn) ? 1 : 0);
    }
}

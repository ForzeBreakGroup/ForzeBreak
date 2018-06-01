using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingDataControlDropdown : SettingDataControlBase
{
    private Dropdown dropdown;
    private string currentValue;

    private void Awake()
    {
        dropdown = GetComponent<Dropdown>();
    }

    public override void Load()
    {
        if (PlayerPrefs.HasKey(settingDataKey))
        {
            currentValue = PlayerPrefs.GetString(settingDataKey);

            for (int i = 0; i < dropdown.options.Count; ++i)
            {
                if (dropdown.options[i].text == currentValue)
                {
                    dropdown.value = i;
                    break;
                }
            }
        }
    }

    public override void Save()
    {
        PlayerPrefs.SetString(settingDataKey, currentValue);
    }

    public void OnValueChangeDropDown(int v)
    {
        currentValue = dropdown.options[v].text;
    }
}

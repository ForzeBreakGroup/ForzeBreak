using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingDataControlInputField : SettingDataControlBase
{
    public override void Load()
    {
        if (PlayerPrefs.HasKey(settingDataKey))
        {
            NetworkManager.instance.ChangePlayerName(PlayerPrefs.GetString(settingDataKey));
        }
    }

    public override void Save()
    {
        PlayerPrefs.SetString(settingDataKey, NetworkManager.instance.playerName);
    }
}

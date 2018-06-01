using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingDataControlBase : MonoBehaviour
{
    [SerializeField]
    protected string settingDataKey;

    public virtual void Save()
    {

    }

    public virtual void Load()
    {

    }
}

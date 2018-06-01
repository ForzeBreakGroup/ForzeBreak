using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingUIControl : MonoBehaviour
{
    [SerializeField]
    private Animator menuAnim;

    SettingDataControlBase[] settingDataControls;

    private void Awake()
    {
        settingDataControls = FindObjectsOfType(typeof(SettingDataControlBase)) as SettingDataControlBase[];
        foreach(SettingDataControlBase control in settingDataControls)
        {
            control.Load();
        }
    }

    public void OnClickReturn()
    {
        GetComponent<Animator>().SetTrigger("Hide");
        menuAnim.SetTrigger("Show");
    }

    public void OnClickApply()
    {
        foreach(SettingDataControlBase control in settingDataControls)
        {
            control.Save();
        }
    }

    private void Update()
    {
        if(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("SettingMenuShow") && Input.GetButtonDown("Cancel"))
        {
            OnClickReturn();
        }
    }
}

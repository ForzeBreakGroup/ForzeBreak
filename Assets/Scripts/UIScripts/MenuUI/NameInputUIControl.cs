using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameInputUIControl : MonoBehaviour
{
    private Animator anim;
    private InputField inputfield;
    private SettingDataControlInputField settingData;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        inputfield = transform.Find("Panel").Find("InputField").GetComponent<InputField>();
        settingData = GetComponent<SettingDataControlInputField>();
        settingData.Load();
    }

    public void OnConfirmNameChange()
    {
        // Change the photon player nickname
        NetworkManager.instance.ChangePlayerName(inputfield.text);
        settingData.Save();

        anim.SetTrigger("Hide");
    }

    public void OnCancelNameChange()
    {
        anim.SetTrigger("Hide");
    }
}

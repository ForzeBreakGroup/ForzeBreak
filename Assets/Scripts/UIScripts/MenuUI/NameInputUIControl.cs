using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameInputUIControl : MonoBehaviour
{
    private Animator anim;
    private InputField inputfield;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        inputfield = transform.Find("Panel").Find("InputField").GetComponent<InputField>();
    }

    public void OnConfirmNameChange()
    {
        // Change the photon player nickname
        NetworkManager.instance.ChangePlayerName(inputfield.text);

        anim.SetTrigger("Hide");
    }

    public void OnCancelNameChange()
    {
        anim.SetTrigger("Show");
    }
}

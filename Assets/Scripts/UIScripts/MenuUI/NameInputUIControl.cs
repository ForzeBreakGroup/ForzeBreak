using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameInputUIControl : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void OnConfirmNameChange()
    {
        // Change the photon player nickname

        anim.SetTrigger("Hide");
    }

    public void OnCancelNameChange()
    {
        anim.SetTrigger("Show");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTransitionUIControl : MonoBehaviour
{
    private enum MenuStates
    {
        Intro,
        Main,
        Setting
    };
    private MenuStates states = MenuStates.Intro;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (states == MenuStates.Intro && Input.anyKey)
        {
            anim.SetTrigger("HideIntro");
        }
    }
}

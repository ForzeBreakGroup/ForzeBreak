using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNameUIControl : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    public void OnClickPlayerNameChange()
    {
        anim.SetTrigger("Show");
    }
}

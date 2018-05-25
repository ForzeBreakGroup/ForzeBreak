using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameUIControl : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    [SerializeField]
    private Color onHoverEnterColor;

    [SerializeField]
    private Color onHoverExitColor;

    public void OnClickPlayerNameChange()
    {
        anim.SetTrigger("Show");
    }

    public void OnPointerEnter()
    {
        GetComponent<Text>().color = onHoverEnterColor;
    }

    public void OnPointerExit()
    {
        GetComponent<Text>().color = onHoverExitColor;
    }
}

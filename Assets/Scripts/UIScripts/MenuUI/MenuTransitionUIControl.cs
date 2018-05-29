using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTransitionUIControl : MonoBehaviour
{
    [SerializeField]
    private Animator menuAnim;

    private void Update()
    {
        if (Input.anyKey)
        {
            UISoundControl.instance.onConfirm();
            menuAnim.SetTrigger("Show");
            gameObject.SetActive(false);
        }
    }
}

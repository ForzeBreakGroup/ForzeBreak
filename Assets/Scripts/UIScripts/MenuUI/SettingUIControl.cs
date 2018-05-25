using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingUIControl : MonoBehaviour
{
    [SerializeField]
    private Animator menuAnim;

    public void OnClickReturn()
    {
        GetComponent<Animator>().SetTrigger("Hide");
        menuAnim.SetTrigger("Show");
    }

    private void Update()
    {
        if(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("SettingMenuShow") && Input.GetButtonDown("Cancel"))
        {
            OnClickReturn();
        }
    }
}

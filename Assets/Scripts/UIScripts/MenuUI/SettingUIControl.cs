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
}

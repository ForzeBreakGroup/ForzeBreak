using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuAnimationControl : MonoBehaviour
{
    [SerializeField]
    private GameObject firstSelectedInMenu;

    public void OnEventStart()
    {
        EventSystem.current.firstSelectedGameObject = null;
    }

    public void OnEventComplete()
    {
        if(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("MainMenuShow"))
        {
            EventSystem.current.firstSelectedGameObject = firstSelectedInMenu;
            EventSystem.current.SetSelectedGameObject(firstSelectedInMenu);
        }
    }
}

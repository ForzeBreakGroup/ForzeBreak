using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWeaponIconControl : MonoBehaviour {

    [SerializeField]
    private Image displayedIcon;
    [SerializeField]
    private Sprite[] resources;


    // Use this for initialization
    void Awake()    
    {
        displayedIcon = GetComponent<Image>();

    }
    

    public void changeIcon(string name)
    {
        if(name==StaticData.CANNON_NAME)
        {
            displayedIcon.sprite = resources[1];
        }
        else if(name == StaticData.MISSILE_NAME)
        {
            displayedIcon.sprite = resources[2];
        }
    }
    
}

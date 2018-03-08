using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 * Author: Robin Zhou
 * 
 * Description:
 * Update the Weapon UI
 */
public class UIWeaponIconControl : MonoBehaviour {

    [SerializeField]
    private Image displayedIcon;
    /// <summary>
    /// Resource for all the image icon, 0 for default
    /// </summary>
    [SerializeField]
    private Sprite[] resources;


    // Use this for initialization
    private void OnEnable()
    {
        displayedIcon = GetComponent<Image>();
        changeIcon("");
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
        else if (name == StaticData.SPIKERAM_NAME)
        {
            displayedIcon.sprite = resources[3];
        }


        else if (name == "")
        {
            displayedIcon.sprite = resources[0];
        }
    }
    
}

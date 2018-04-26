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
public class UIWeaponIconControl : UIControl
{

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
        ChangeIcon("");
    }

    public void ChangeIcon(string name)
    {
        if(name == StaticData.CANNON_COMPONENT)
        {
            displayedIcon.sprite = resources[1];
        }
        else if (name == StaticData.SPIKERAM_COMPONENT)
        {
            displayedIcon.sprite = resources[2];
        }
        else if (name == StaticData.HOTPOTATO_COMPONENT)
        {
            displayedIcon.sprite = resources[3];
        }
        else if (name == StaticData.MORNINGSTAR_COMPONENT)
        {
            displayedIcon.sprite = resources[4];
        }
        else if (name == StaticData.PUNCHINGGLOVE_COMPONENT)
        {
            displayedIcon.sprite = resources[5];
        }
        else if (name == StaticData.SNOWBALL_COMPONENT)
        {
            displayedIcon.sprite = resources[6];
        }
        else if (name == StaticData.BANNANTRAP_COMPONENT)
        {
            displayedIcon.sprite = resources[7];
        }
        else if (name == StaticData.SPRINGTRAP_COMPONENT)
        {
            displayedIcon.sprite = resources[8];
        }
        else if (name == "")
        {
            displayedIcon.sprite = resources[0];
        }
    }

    public override void EnableUIControl()
    {
        base.EnableUIControl();
    }
}

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
public class UIPowerUpIconManager : UIControl
{
    private Animator iconAnim;
    private Image displayedIcon;

    [SerializeField]
    private Sprite defaultIconImage;

    private static UIPowerUpIconManager powerUpIconManager;
    public static UIPowerUpIconManager instance
    {
        get
        {
            if (!powerUpIconManager)
            {
                powerUpIconManager = FindObjectOfType(typeof(UIPowerUpIconManager)) as UIPowerUpIconManager;
                if (!powerUpIconManager)
                {
                    Debug.LogError("UIPowerUpIconManager script must be attached to an active GameObject in scene");
                }
                else
                {
                    powerUpIconManager.Init();
                }
            }

            return powerUpIconManager;
        }
    }

    private void Init()
    {
        displayedIcon = GetComponent<Image>();
        iconAnim = GetComponent<Animator>();
    }

    public void ChangeIcon(Sprite img = null)
    {
        if (img == null)
        {
            displayedIcon.sprite = defaultIconImage;
        }
        else
        {
            Debug.Log(img.name);
            displayedIcon.sprite = img;
            iconAnim.SetTrigger("PlayIconAnim");
        }
    }

    public override void EnableUIControl()
    {
        base.EnableUIControl();
    }
}

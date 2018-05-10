using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 * Author: Robin Zhou
 * 
 * Description:
 * Update the damage UI
 */
public class UIDamageIconControl : UIControl
{
    [SerializeField]
    private Image displayedIcon;

    [SerializeField]
    private Sprite[] resources;

    private DamageSystem damageSystem;
    // Use this for initialization

    private void OnEnable()
    {
        displayedIcon = GetComponent<Image>();
    }

    protected override void UpdateUIControl()
    {
        base.UpdateUIControl();
        // displayedIcon.sprite = resources[(int)damageSystem.GetDamageThreshold()];
    }

    public override void EnableUIControl()
    {
        base.EnableUIControl();
        damageSystem = NetworkManager.instance.GetLocalPlayer().GetComponent<DamageSystem>();
    }
}

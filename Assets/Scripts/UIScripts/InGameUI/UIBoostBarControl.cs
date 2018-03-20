using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 * Author: Robin Zhou
 * 
 * Description:
 * Update the boost UI
 */
public class UIBoostBarControl : UIControl
{
    Image progressBar;
    Text percentage;

    BoostControl bc;

    void Awake()
    {
        progressBar = transform.Find("ProgressBar").gameObject.GetComponent<Image>();
    }

    public override void EnableUIControl()
    {
        base.EnableUIControl();
        bc = NetworkManager.instance.GetLocalPlayer().GetComponent<BoostControl>();
    }

    protected override void UpdateUIControl()
    {
        base.UpdateUIControl();
        progressBar.fillAmount = bc.energy / bc.maxEnergy;
    }
}

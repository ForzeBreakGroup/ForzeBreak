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


    void Awake()
    {
        progressBar = transform.Find("ProgressBar").gameObject.GetComponent<Image>();
    }


    public void updateProgressBar(float progress)
    {
        base.UpdateUIControl();
        progressBar.fillAmount = progress;
    }
}

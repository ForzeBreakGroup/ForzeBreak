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
public class UIBoostBarControl : MonoBehaviour {

    Image progressBar;
    Text percentage;

    BoostControl bc;

    void OnEnable()
    {
        progressBar = transform.Find("ProgressBar").gameObject.GetComponent<Image>();
        percentage = transform.Find("Text").gameObject.GetComponent<Text>();
        bc = NetworkManager.localPlayer.GetComponent<BoostControl>();
    }

    void Update()
    {
        progressBar.fillAmount = bc.energy/bc.maxEnergy;

        percentage.text = ""+(int)bc.energy / bc.maxEnergy*100+"%";

    }
}

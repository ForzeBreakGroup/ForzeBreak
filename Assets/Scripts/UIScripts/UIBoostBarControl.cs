using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBoostBarControl : MonoBehaviour {

    Image progressBar;
    Text percentage;

    BoostControl bc;

    // Use this for initialization
    void Awake()
    {
        progressBar = transform.Find("ProgressBar").gameObject.GetComponent<Image>();
        percentage = transform.Find("Text").gameObject.GetComponent<Text>();
        bc = NetworkManager.localPlayer.GetComponent<BoostControl>();
    }

    // Update is called once per frame
    void Update()
    {
        progressBar.fillAmount = bc.energy/bc.maxEnergy;

        percentage.text = ""+(int)bc.energy / bc.maxEnergy*100+"%";

    }
}

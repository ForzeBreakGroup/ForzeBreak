using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour {
    

    private Image speedBar;
    private Image boostBar;

    // Use this for initialization
    void Start()
    {
        speedBar = GameObject.FindGameObjectWithTag("UI_SpeedBar").GetComponent<Image>();
        boostBar = GameObject.FindGameObjectWithTag("UI_BoostBar").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        float speedBarProgress = GetComponent<Rigidbody>().velocity.magnitude / 25;
        speedBar.fillAmount = speedBarProgress;
        float boostBarProgress = GetComponent<BoostControl>().energy / 100;
        boostBar.fillAmount = boostBarProgress;
    }
}

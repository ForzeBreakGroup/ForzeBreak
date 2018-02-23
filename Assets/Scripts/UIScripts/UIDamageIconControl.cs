using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDamageIconControl : MonoBehaviour {


    [SerializeField]
    private Image displayedIcon;
    [SerializeField]
    private Sprite[] resources;

    private DamageSystem damageSystem;
    // Use this for initialization

    private void OnEnable()
    {
        displayedIcon = GetComponent<Image>();

        damageSystem = NetworkManager.localPlayer.GetComponent<DamageSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(damageSystem.damageAmplifyPercentage<200f)
        {
            displayedIcon.sprite = resources[0];
        }
        else if(damageSystem.damageAmplifyPercentage < 300f)
        {
            displayedIcon.sprite = resources[1];

        }
        else if (damageSystem.damageAmplifyPercentage < 400f)
        {
            displayedIcon.sprite = resources[2];

        }
        else if (damageSystem.damageAmplifyPercentage < 500f)
        {

            displayedIcon.sprite = resources[3];
        }
    }
}

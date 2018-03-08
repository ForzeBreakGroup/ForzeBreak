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
        displayedIcon.sprite = resources[(int)damageSystem.GetDamageThreshold()];
    }
}

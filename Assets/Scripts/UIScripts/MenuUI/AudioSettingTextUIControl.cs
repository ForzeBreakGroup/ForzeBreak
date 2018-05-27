using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingTextUIControl : MonoBehaviour
{
    Text txt;
    Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        txt = transform.GetComponentInChildren<Text>();
    }

    private void Update()
    {
        txt.text = slider.value.ToString();
    }
}

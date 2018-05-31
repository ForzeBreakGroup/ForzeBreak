using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehicleStatsUI : MonoBehaviour
{
    private enum VehicleStatsType
    {
        Speed,
        Weight,
        Control,
        Energy,
        Description
    };

    private Image[] images;
    private Text desc;

    [SerializeField]
    private VehicleStatsType type;

    [SerializeField]
    private Sprite activeBarImg;

    [SerializeField]
    private Sprite inactiveBarImg;

    private void Awake()
    {
        images = GetComponentsInChildren<Image>();
        desc = GetComponentInChildren<Text>();
    }

    private void OnEnable()
    {
        VehicleSelectionControl.VehicleStats vs = VehicleSelectionControl.instance.GetSelectVehicleStats();
        Debug.Log(vs.VehicleSpeed);
        switch (type)
        {
            case VehicleStatsType.Speed:
                UpdateStats(vs.VehicleSpeed);
                break;
            case VehicleStatsType.Weight:
                UpdateStats(vs.VehicleWeight);
                break;
            case VehicleStatsType.Control:
                UpdateStats(vs.VehicleControl);
                break;
            case VehicleStatsType.Energy:
                UpdateStats(vs.VehicleEnergy);
                break;
            case VehicleStatsType.Description:
                UpdateStats(vs.VehicleDescription);
                break;
        }
    }

    private void UpdateStats(int v)
    {
        for(int i = 0; i < v; ++i)
        {
            images[i].sprite = activeBarImg;
        }

        for (int i = v; i < images.Length; ++i)
        {
            images[i].sprite = inactiveBarImg;
        }
    }

    private void UpdateStats(string s)
    {
        desc.text = s;
    }
}

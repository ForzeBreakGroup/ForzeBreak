﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehicleSelectionControl : MonoBehaviour
{
    [System.Serializable]
    public struct VehicleStats
    {
        [Range(1, 5)]
        [SerializeField]
        public int VehicleSpeed;

        [Range(1, 5)]
        [SerializeField]
        public int VehicleWeight;

        [Range(1, 5)]
        [SerializeField]
        public int VehicleControl;

        [Range(1, 5)]
        [SerializeField]
        public int VehicleEnergy;

        [SerializeField]
        public string VehicleDescription;
    }


    [System.Serializable]
    private struct VehicleSet
    {
        [SerializeField]
        public string VehicleName;
        [SerializeField]
        public string VehicleDisplayDame;
        [SerializeField]
        public GameObject VehicleModel;

        [SerializeField]
        public VehicleStats VehicleStats;
    }

    private int currentIndex = 0;
    public bool activeForSelection = true;
    public Text VehicleNameUI;

    [SerializeField]
    private VehicleSet[] VehicleList;

    public static VehicleSelectionControl vehicleSelectionControl;
    public static VehicleSelectionControl instance
    {
        get
        {
            if (!vehicleSelectionControl)
            {
                vehicleSelectionControl = FindObjectOfType<VehicleSelectionControl>();
                if (!vehicleSelectionControl)
                {
                    Debug.LogError("LobbyManager script must be attached to an active gameobject in scene");
                }
            }

            return vehicleSelectionControl;
        }
    }

    private void Awake()
    {
        VehicleList[currentIndex].VehicleModel.SetActive(true);
        VehicleNameUI.text = VehicleList[currentIndex].VehicleDisplayDame;
    }

    public void NextVehicle()
    {
        if (activeForSelection)
        {
            VehicleList[currentIndex].VehicleModel.SetActive(false);
            currentIndex = (currentIndex + 1) % VehicleList.Length;
            VehicleList[currentIndex].VehicleModel.SetActive(true);

            VehicleNameUI.text = VehicleList[currentIndex].VehicleDisplayDame;
        }
    }

    public void PreviousVehicle()
    {
        if(activeForSelection)
        {
            VehicleList[currentIndex].VehicleModel.SetActive(false);
            currentIndex--;
            if (currentIndex < 0)
                currentIndex += VehicleList.Length;
            VehicleList[currentIndex].VehicleModel.SetActive(true);

            VehicleNameUI.text = VehicleList[currentIndex].VehicleDisplayDame;
        }
    }

    public string GetSelectVehicle()
    {
        activeForSelection = false;
        return VehicleList[currentIndex].VehicleName;
    }

    public VehicleStats GetSelectVehicleStats()
    {
        return VehicleList[currentIndex].VehicleStats;
    }

}

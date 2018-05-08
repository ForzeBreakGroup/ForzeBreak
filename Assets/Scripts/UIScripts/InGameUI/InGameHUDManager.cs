using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameHUDManager : MonoBehaviour
{
    private UIBoostBarControl boostBarControl;
    //private UIDamageIconControl damageIconControl;
    private UIWeaponIconControl weaponIconControl;
    private UINetworkTimerControl networkTimerControl;
    private UIReticleControl reticleControl;

	private UIKOControl koControl;

    private static InGameHUDManager inGameHUDManager;
    public static InGameHUDManager instance
    {
        get
        {
            if (!inGameHUDManager)
            {
                inGameHUDManager = FindObjectOfType<InGameHUDManager>();
                if (!inGameHUDManager)
                {
                    Debug.LogError("InGameHUDManager script must be attached to an active gameobject in scene");
                }
                else
                {
                    inGameHUDManager.Init();
                }
            }

            return inGameHUDManager;
        }
    }

    private void Init()
    {
        boostBarControl = FindObjectOfType<UIBoostBarControl>();
        //damageIconControl = FindObjectOfType<UIDamageIconControl>();
        weaponIconControl = FindObjectOfType<UIWeaponIconControl>();
        networkTimerControl = FindObjectOfType<UINetworkTimerControl>();
        reticleControl = FindObjectOfType<UIReticleControl>();
        //koControl = FindObjectOfType<UIKOControl> ();
    }

    public void EnableInGameHUD()
    {
        // Enable each individual UI control
        boostBarControl.EnableUIControl();
        //damageIconControl.EnableUIControl();
        weaponIconControl.EnableUIControl();
        networkTimerControl.EnableUIControl();


        // Disable Room waiting UI
    }

    public void UpdateWeaponIcon(string powerUpName)
    {
        weaponIconControl.ChangeIcon(powerUpName);
    }

    public void UpdateBoostBar(float progress)
    {
        boostBarControl.updateProgressBar(progress);
    }

    public void ReticleUIDisplay(ReticleUIState state, Vector3 pos)
    {
        reticleControl.DisplayReticleUI(state, pos);
    }

	public void ShowKOUI()
	{
		koControl.setUIActive ();
	}
}

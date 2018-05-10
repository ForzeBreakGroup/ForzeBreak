using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 * Author: Robin Zhou
 * 
 * Description:
 * Update the Weapon UI
 */
public class UIPowerUpIconManager : MonoBehaviour
{
    private Animator anim;
    private Image displayedIcon;
    private Vector3 pos;
    private RectTransform rectTransform;

    [SerializeField]
    private Sprite defaultIconImage;

    private static UIPowerUpIconManager powerUpIconManager;
    public static UIPowerUpIconManager instance
    {
        get
        {
            if (!powerUpIconManager)
            {
                powerUpIconManager = FindObjectOfType(typeof(UIPowerUpIconManager)) as UIPowerUpIconManager;
                if (!powerUpIconManager)
                {
                    Debug.LogError("UIPowerUpIconManager script must be attached to an active GameObject in scene");
                }
                else
                {
                    powerUpIconManager.Init();
                }
            }


            return powerUpIconManager;
        }
    }

    private void Init()
    {
        displayedIcon = GetComponent<Image>();
        anim = GetComponent<Animator>();
        rectTransform = GetComponent<RectTransform>();
        pos = rectTransform.anchoredPosition;
        StartCoroutine(AnimateIconPosition());
    }

    public void ChangeIcon(Sprite img = null)
    {
        if (img == null)
        {
            displayedIcon.sprite = defaultIconImage;
        }
        else
        {
            displayedIcon.sprite = img;
            anim.SetTrigger("PlayIconAnim");
            rectTransform.anchoredPosition = new Vector3(-Screen.width / 2, Screen.height / 2, 0);
        }
    }

    IEnumerator AnimateIconPosition()
    {
        while(true)
        {
            yield return null;

            rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, pos, 0.05f);
        }
    }
}

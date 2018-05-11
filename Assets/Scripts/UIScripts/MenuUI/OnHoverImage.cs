using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnHoverImage : MonoBehaviour
{
    Image image;

    [SerializeField]
    private Color hoverEnterColor;

    [SerializeField]
    private Color hoverExitColor;

    private Color currentColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        currentColor = hoverExitColor;
    }

    public void OnHoverEnter()
    {
        currentColor = hoverEnterColor;
    }

    public void OnHoverExit()
    {
        currentColor = hoverExitColor;
    }

    private void Update()
    {
        image.color = Color.Lerp(image.color, currentColor, 0.1f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnHoverText : MonoBehaviour
{
    Text text;

    [SerializeField]
    private Color hoverEnterColor;

    [SerializeField]
    private Color hoverExitColor;

    private Color currentColor;

    private void Awake()
    {
        text = transform.Find("Text").GetComponent<Text>();
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
        text.color = Color.Lerp(text.color, currentColor, 0.1f);
    }
}

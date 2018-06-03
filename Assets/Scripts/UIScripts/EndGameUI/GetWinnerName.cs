using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetWinnerName : MonoBehaviour
{
    private Text winnerName;
    private Animator anim;

    private void Awake()
    {
        winnerName = GetComponentInChildren<Text>();
        anim = GetComponent<Animator>();
    }

    public void OnDisplayWinnerName(string winnerName)
    {
        this.winnerName.text = winnerName;
        anim.SetTrigger("Show");
    }
}

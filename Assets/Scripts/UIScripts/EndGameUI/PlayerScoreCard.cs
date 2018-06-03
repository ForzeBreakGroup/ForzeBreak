using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreCard : MonoBehaviour
{
    [SerializeField]
    private Sprite frontSide;

    [SerializeField]
    private Sprite backSide;

    private bool isBackside = true;
    private Image img;
    private Animator anim;

    private GameObject playerName;
    private GameObject playerScore;

    private string playerNameTemp;
    private string scoreTemp;

    private void Awake()
    {
        img = GetComponent<Image>();
        anim = GetComponent<Animator>();

        playerName = transform.Find("PlayerName").gameObject;
        playerScore = transform.Find("Score").gameObject;
    }

    public void Flip()
    {
        anim.SetTrigger("Flip");
    }

    public void OnFlipTo90Degree()
    {
        img.sprite = (isBackside) ? frontSide : backSide;
        isBackside = !isBackside;
        EnableTextDisplay();
    }

    private void EnableTextDisplay()
    {
        playerName.SetActive(!isBackside);
        playerScore.SetActive(!isBackside);

        playerName.GetComponent<Text>().text = playerNameTemp;
        playerScore.GetComponent<Text>().text = scoreTemp;
    }

    public void SetPlayerInfo(string playerName, int playerScore)
    {
        playerNameTemp = playerName;
        scoreTemp = playerScore.ToString();
    }
}

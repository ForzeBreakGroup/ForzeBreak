using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusEntry : MonoBehaviour
{
    Text playerName;
    Text status;

    private void Awake()
    {
        playerName = transform.Find("Name").GetComponent<Text>();
        status = transform.Find("Ready").GetComponent<Text>();
    }

    public void UpdateStatus(string name, bool status)
    {
        playerName.text = name;
        this.status.text = (status) ? "Ready" : "Waiting";
        this.status.color = (status) ? Color.green : Color.red;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LobbyUIControllerSupport : MonoBehaviour
{
    [SerializeField]
    private UnityEvent evt;

    [SerializeField]
    private string inputName;

    private void Awake()
    {
        if (evt == null)
        {
            evt = new UnityEvent();
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown(inputName))
        {
            evt.Invoke();
        }
    }
}

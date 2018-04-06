using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIControl : MonoBehaviour
{
    #region OnClick Event
    public void OnClickPlayerReady()
    {
        LobbyManager.instance.OnPlayerClickReady();
    }
    #endregion
}

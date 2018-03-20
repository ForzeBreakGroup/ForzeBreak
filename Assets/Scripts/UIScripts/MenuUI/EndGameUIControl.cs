using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameUIControl : MonoBehaviour
{
    public void OnClickRematch()
    {
        EndGameLobbyManager.instance.RegisterForRematch(true);
    }

    public void OnClickExitToMenu()
    {
        EndGameLobbyManager.instance.RegisterForRematch(false);
    }
}

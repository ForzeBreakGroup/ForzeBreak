using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Provide services to menu scene button events
 */
public class MenuUIControl : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    #region OnClick Events
    public void OnClickLocalSplitScreen()
    {
        NetworkManager.instance.StartSplitScreen();
    }

    public void OnClickOnlineSession()
    {
        anim.SetTrigger("OnlineSubMenu");
    }

    public void OnClickMatchMaking()
    {
        NetworkManager.instance.StartMatchMaking();
    }

    public void OnClickRefreshRoomList()
    {
        NetworkManager.instance.RefreshCustomRoomList();
    }

    public void OnClickCustomRoomOption()
    {

    }

    public void OnClickJoinCustomRoomOption(string name)
    {
        NetworkManager.instance.JoinRoomByName(name);
    }

    public void OnClickQuitApplication()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
    #endregion
}

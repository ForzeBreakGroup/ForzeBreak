using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsUIControl : MonoBehaviour
{
    [SerializeField]
    private string returnSceneName;

    public void OnClickReturn()
    {
        SceneManager.LoadScene(returnSceneName);
    }
}

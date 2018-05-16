using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticManagerTest : MonoBehaviour
{
    private void Awake()
    {
        AnalyticManager.Write();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemAutoDestroy : MonoBehaviour
{
    ParticleSystem[] particleSystems;
    private void Awake()
    {
        float longestPSDuration = 0.0f;
        particleSystems = GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem ps in particleSystems)
        {
            float duration = ps.main.startLifetime.constantMax + ps.main.duration;
            if (duration > longestPSDuration)
            {
                longestPSDuration = duration;
            }
        }
    }

    private void Update()
    {
        Debug.Log(GetComponent<ParticleSystem>().IsAlive(true));
    }
}

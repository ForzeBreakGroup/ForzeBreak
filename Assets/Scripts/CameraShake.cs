using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraShake : MonoBehaviour
{
    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.
    public Transform camTransform;

    // How long the object should shake for.
    public static float shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public static float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    Vector3 originalPos;

    void Awake()
    {
        if (camTransform == null)
        {
            camTransform = transform;
        }
    }

    void OnEnable()
    {
        originalPos = camTransform.localPosition;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeAmount = 0.7f;
            decreaseFactor = 1.0f;
            shakeDuration = 0f;
            camTransform.localPosition = originalPos;
        }
    }

    public static void Shake(float time)
    {
        shakeDuration = time;
    }

    public static void Shake()
    {
        shakeDuration = 0.2f;
    }

    public static void Shake(float time, float intensity)
    {
        shakeDuration = time;
        shakeAmount = intensity;
    }
}
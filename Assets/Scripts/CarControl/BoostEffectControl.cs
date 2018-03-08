using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
/*
 * Author: Robin
 * 
 * Description:
 * Control the physical boosting effect.
 */
class BoostEffectControl : Photon.MonoBehaviour
{
    public ParticleSystem[] EngineParticleSystems;
    public float[] MaxParticleSystemsAlpha;
    public PostProcessingProfile PPProfile;

    private const float SpeedIntencity = 2;
    private readonly List<Material> _enginePsMaterials = new List<Material>();
    private float _speed;


    private void Awake()
    {
        enabled = transform.root.gameObject.GetPhotonView().isMine;
        foreach (ParticleSystem engineParticleSystem in EngineParticleSystems)
            _enginePsMaterials.Add(engineParticleSystem.GetComponent<Renderer>().material);

        UpdateColorBySpeed(false);

    }

    public void UpdateColorBySpeed(bool isBoostHold)
    {
        float speed = SpeedIntencity * Time.deltaTime;

        if (isBoostHold)
            _speed += speed;
        else
            _speed -= speed;

        _speed = Mathf.Clamp01(_speed);


        for (int i = 0; i < _enginePsMaterials.Count; i++)
        {
            var tintColor = _enginePsMaterials[i].GetColor("_TintColor");

            tintColor.a = Mathf.Clamp(_speed, 0, MaxParticleSystemsAlpha[i]);

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_enginePsMaterials[i].HasProperty("_ColorEdge"))
            {
                tintColor.r = Mathf.Clamp(_speed, 0, MaxParticleSystemsAlpha[i]);
                tintColor.g = Mathf.Clamp(_speed, 0, MaxParticleSystemsAlpha[i]);
                tintColor.b = Mathf.Clamp(_speed, 0, MaxParticleSystemsAlpha[i]);
            }

            _enginePsMaterials[i].SetColor("_TintColor", tintColor);
        }
    }
}

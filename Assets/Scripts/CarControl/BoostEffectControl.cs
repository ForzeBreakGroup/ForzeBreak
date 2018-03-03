using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

class BoostEffectControl : Photon.MonoBehaviour
{
    private const float SpeedIntencity = 2;

    public ParticleSystem[] EngineParticleSystems;
    public float[] MaxParticleSystemsAlpha;
    public PostProcessingProfile PPProfile;

    private readonly List<Material> _enginePsMaterials = new List<Material>();
    private float _speed;
    private bool _isButtonHold;
    private int playerNum;

    private void Awake()
    {
        enabled = transform.root.gameObject.GetPhotonView().isMine;
        foreach (ParticleSystem engineParticleSystem in EngineParticleSystems)
            _enginePsMaterials.Add(engineParticleSystem.GetComponent<Renderer>().material);

        playerNum = transform.root.GetComponent<CarUserControl>().playerNum;
        UpdateColorBySpeed();

    }

    private void Update()
    {
        float speed = SpeedIntencity * Time.deltaTime;
        
        if (Input.GetButtonDown("Boost_Controller" + playerNum)|| Input.GetButtonDown("Boost_Mouse"))
        {
            _isButtonHold = true;
        }
        else if (Input.GetButtonUp("Boost_Controller" + playerNum) || Input.GetButtonUp("Boost_Mouse"))
        {
            _isButtonHold = false;

        }

        if (_isButtonHold)
            _speed += speed;
        else
            _speed -= speed;

        _speed = Mathf.Clamp01(_speed);

        UpdateColorBySpeed();
    }

    private void UpdateColorBySpeed()
    {
        for (int i = 0; i < _enginePsMaterials.Count; i++)
        {
            var tintColor = _enginePsMaterials[i].GetColor("_TintColor");

            tintColor.a = Mathf.Clamp(_speed, 0, MaxParticleSystemsAlpha[i]);

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            // sparks
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

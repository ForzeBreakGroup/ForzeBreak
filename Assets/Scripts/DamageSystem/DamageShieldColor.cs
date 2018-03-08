using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageShieldColor : MonoBehaviour
{
    private DamageSystem damageSystem;
    private Material shieldMaterial;

    [SerializeField]
    private Color[] shieldColorThreshold = new Color[4];

    // Use this for initialization
    private void Awake()
    {
        damageSystem = transform.root.gameObject.GetComponent<DamageSystem>();
        shieldMaterial = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update ()
    {
        int damageCondition = (int)damageSystem.GetDamageThreshold();

        Color lerpColor = Color.Lerp(shieldMaterial.GetColor("_TintColor"), shieldColorThreshold[damageCondition], Time.deltaTime);
        shieldMaterial.SetColor("_TintColor", lerpColor);
    }
}

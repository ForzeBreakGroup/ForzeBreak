using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Jason Lin
 * 
 * Description:
 * Color indication of player vehicle's damage condition
 */
public class DamageShieldColor : MonoBehaviour
{
    #region Private Members
    /// <summary>
    /// Internal reference to DamageSystem component attached to player object
    /// </summary>
    private DamageSystem damageSystem;

    /// <summary>
    /// Internal refrence to the material component attached to the shield
    /// </summary>
    private Material shieldMaterial;

    /// <summary>
    /// Adjustable 4 stages of color indication for 4 stages of damage threshold
    /// </summary>
    [SerializeField]
    private Color[] shieldColorThreshold = new Color[4];
    #endregion

    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        damageSystem = transform.root.gameObject.GetComponent<DamageSystem>();
        shieldMaterial = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        int damageCondition = (int)damageSystem.GetDamageThreshold();

        Color lerpColor = Color.Lerp(shieldMaterial.GetColor("_TintColor"), shieldColorThreshold[damageCondition], Time.deltaTime);
        shieldMaterial.SetColor("_TintColor", lerpColor);
    }

    #endregion
}

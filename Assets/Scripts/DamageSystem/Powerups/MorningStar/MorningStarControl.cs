using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorningStarControl : PowerUpBase {


    public override void AdjustModel()
    {
        transform.localPosition = componentOffset;
        transform.localRotation = Quaternion.Euler(componentAngle);

        Transform t = transform.Find("joint (10)");
        ConfigurableJoint joint = t.GetComponent<ConfigurableJoint>();
        joint.connectedBody = transform.root.GetComponent<Rigidbody>();
    }
}

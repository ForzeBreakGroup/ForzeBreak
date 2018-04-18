using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IComponentCollision
{
    void ComponentCollision(Collision collision);
    void ComponentTrigger(Collider other);
}

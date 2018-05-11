using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPInstantiateEffect : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<PSMeshRendererUpdater>().MeshObject = transform.root.gameObject;
        GetComponent<PSMeshRendererUpdater>().UpdateMeshEffect();
    }
}

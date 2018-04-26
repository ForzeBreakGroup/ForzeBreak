using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameDisplay : MonoBehaviour
{
    TextMesh playerName;
    Camera cam;

    private void Awake()
    {
        playerName = GetComponent<TextMesh>();
        cam = NetworkManager.instance.GetPlayerCamera();
        PhotonView view = transform.root.gameObject.GetPhotonView();
        PhotonPlayer p = PhotonPlayer.Find(view.ownerId);

        playerName.text = p.NickName;
        float[] serializeColor = PhotonPlayer.Find(view.ownerId).CustomProperties["Color"] as float[];
        Color c = new Color(serializeColor[0], serializeColor[1], serializeColor[2], serializeColor[3]);
        playerName.color = c;
    }

    private void Update()
    {
        Vector3 v = cam.transform.position - transform.position;
        v.x = v.z = 0.0f;
        transform.LookAt(cam.transform.position - v);
        transform.Rotate(0, 180.0f, 0);
    }
}

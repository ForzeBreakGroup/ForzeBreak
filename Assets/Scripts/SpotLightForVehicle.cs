using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightForVehicle : MonoBehaviour {


    Transform[] lightingPods = new Transform[4];

    private void Start()
    {
        for(int i=0; i<transform.childCount; i++)
        {
            lightingPods[i] = transform.GetChild(i);
            lightingPods[i].gameObject.layer = 8 + i;
        }
    }

    // Update is called once per frame
    void Update () {
        GameObject[] cars =  GameObject.FindGameObjectsWithTag("Player");
        GameObject[] cameras = GameObject.FindGameObjectsWithTag("MainCamera");
        for(int i=0; i<cars.Length; i++)
        {
            if(i<cameras.Length)
            {
                cameras[i].GetComponent<Camera>().cullingMask = ~(1 << (8 + i));
            }

            LineRenderer line = lightingPods[i].GetComponent<LineRenderer>();
            if (line != null)
            {
                if (line.enabled == false)
                    line.enabled = true;
                Vector3[] positions = { lightingPods[i].position, cars[i].transform.position};
                line.SetPositions(positions);

            }

        }
	}
}

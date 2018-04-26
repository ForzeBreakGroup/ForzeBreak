using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightForVehicle : MonoBehaviour {

    public int playerID = 0;
    LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update () {
        GameObject[] cars = GameObject.FindGameObjectsWithTag("Player");
        for (int i=0; i< cars.Length; i++)
        {
            if(cars[i].GetComponent<PhotonView>().ownerId == playerID)
            {
                if (line != null)
                {
                    if (line.enabled == false)
                        line.enabled = true;
                    Vector3[] positions = { transform.position, cars[i].transform.position };
                    line.SetPositions(positions);
                }


            }
        }
	}
}

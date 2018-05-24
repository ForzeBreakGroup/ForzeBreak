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
        if (line != null)
        {

            GameObject target = getTarget(playerID);
            if (target!=null)
            {
                if (line.enabled == false)
                    line.enabled = true;
                Vector3[] positions = { transform.position, target.transform.position};
                line.SetPositions(positions);
            }
            else
            {
                line.enabled = false;
            }
        }
    }

    GameObject getTarget(int ID)
    {
        GameObject[] cars = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < cars.Length; i++)
        {
            if (cars[i].GetComponent<PhotonView>().ownerId == playerID)
            {
                return cars[i];
            }
        }
        return null;
    }
}

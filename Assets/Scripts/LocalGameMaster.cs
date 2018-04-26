using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalGameMaster : MonoBehaviour
{

    [SerializeField] private GameObject camPrefeb;
    [SerializeField] private GameObject carPrefeb;

    [Range(1, 4, order = 1)]
    [SerializeField]
    private int playerAmount = 1;

    private Transform spawnPoint;

    // Use this for initialization
    void Start()
    {
        foreach(string name in Input.GetJoystickNames())
        {
            Debug.Log(name);
        }
        switch(playerAmount)
        {
            case 1:
                for (int i = 0; i < playerAmount; i++)
                {
                    spawnPoint = transform.Find("SpawnPoints").GetChild(i);
                    GameObject car = Instantiate(carPrefeb, spawnPoint.position, spawnPoint.rotation);
                    GameObject cam = Instantiate(camPrefeb, spawnPoint.position, spawnPoint.rotation);
                    cam.GetComponent<CameraControl>().target = car;
                }
                break;

            case 2:
                for (int i = 0; i < playerAmount; i++)
                {
                    spawnPoint = transform.Find("SpawnPoints").GetChild(i);
                    Debug.Log(spawnPoint.name);
                    GameObject car = Instantiate(carPrefeb, spawnPoint.position, spawnPoint.rotation);
                    car.GetComponent<CarUserControl>().controllerNum = i+1;
                    GameObject cam = Instantiate(camPrefeb, spawnPoint.position, spawnPoint.rotation);
                    cam.GetComponent<CameraControl>().target = car;
                    float marginX = (i > 1) ? (i - 2) * 0.5f : i * 0.5f;
                    float marginY = (i > 1) ? 0.5f: 0f;
                    
                    cam.GetComponentInChildren<Camera>().rect = new Rect(marginX, marginY, 0.5f, 1f);

                }
                break;
            default:
                for (int i = 0; i < playerAmount; i++)
                {
                    spawnPoint = transform.Find("SpawnPoints").GetChild(i);
                    GameObject car = Instantiate(carPrefeb, spawnPoint.position, spawnPoint.rotation);
                    car.GetComponent<CarUserControl>().controllerNum = i + 1;
                    GameObject cam = Instantiate(camPrefeb, spawnPoint.position, spawnPoint.rotation);
                    cam.GetComponent<CameraControl>().target = car;
                    float marginX = (i > 1) ? (i - 2) * 0.5f : i * 0.5f;
                    float marginY = (i > 1) ?  0f : 0.5f;

                    cam.GetComponentInChildren<Camera>().rect = new Rect(marginX, marginY, 0.5f, 0.5f);

                }
                break;

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
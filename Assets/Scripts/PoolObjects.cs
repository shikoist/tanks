using UnityEngine;
using System.Collections;

public class PoolObjects : MonoBehaviour {
    public int numberObjectsInPool = 10;
    public Transform objectTransform;

    public Transform[] poolObjects;

    private MainScript ms;

	// Use this for initialization
	void Start () 
    {
        ms = GameObject.Find("MainScript").GetComponent<MainScript>();

        poolObjects = new Transform[numberObjectsInPool];

        for (int i = 0; i < numberObjectsInPool; i++)
        {
            poolObjects[i] = (Transform)Instantiate(objectTransform, Vector3.zero, Quaternion.identity);
            poolObjects[i].parent = transform;
            poolObjects[i].gameObject.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public GameObject ShowAtPosition(Vector3 pos)
    {
        for (int i = 0; i < numberObjectsInPool; i++)
        {
            if (poolObjects[i].gameObject.activeSelf == false)
            {
                poolObjects[i].gameObject.SetActive(true);
                if (ms.cameraMode == 0)
                {
                    poolObjects[i].rotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
                }
                else if (ms.cameraMode == 1)
                {
                    poolObjects[i].rotation = Quaternion.Euler(-35.0f, 180.0f, 0.0f);
                }
                else if (ms.cameraMode == 2)
                {
                    poolObjects[i].rotation = Quaternion.Euler(-45.0f, 180.0f + 27.5f, 0.0f);
                }
                poolObjects[i].position = pos;
                return poolObjects[i].gameObject;
            }
        }
        Debug.Log(Time.time.ToString() + " : Нет свободных обьектов в пуле " + gameObject.name);
        //Debug.Break();
        return null;
    }
}

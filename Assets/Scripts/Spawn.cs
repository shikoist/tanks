using UnityEngine;
using System.Collections;

public class Spawn : MonoBehaviour
{
    public int r = 0;
    public string postSpawn;
    public Transform shield;
    public int number;
    public int type;

    private float timer;
    private float rateTimer = 1.0f;
    private MainScript ms;
    private bool oldPause;
    private Color[] colors = new Color[4] {
        Color.yellow,
        Color.green,
        Color.red,
        Color.blue 
    };

    private void Start()
    {
        ms = GameObject.Find("MainScript").GetComponent<MainScript>();
        timer = ms.otherTime + rateTimer;
    }

    private void Update()
    {
        if (ms.pause != oldPause)
        {
            oldPause = ms.pause;
            if (oldPause)
            {
                OnPause();
            }
            else
            {
                OnResume();
            }
        }

        if (Network.isServer)
        {
            if (ms.otherTime > timer)
            {
                if (number >= 4)
                {
                    ms.nv.RPC("SpawnEnemy", RPCMode.All, r, number, type);
                }
                else
                {
                    Debug.Log("RPC SpawnPlayer was sent.");
                    ms.nv.RPC("SpawnPlayer", RPCMode.All, number);
                }
                timer = Mathf.Infinity;
                gameObject.SetActive(false);
            }
        }
    }
    
    void OnEnable()
    {
        Debug.Log(Time.time + " : Spawn " + number + " was enabled.");
        if (ms != null)
        {
            timer = ms.otherTime + rateTimer;
        }
    }

    private void OnPause()
    {
        //animation["SpawnAnimation"].speed = 0.0f;
    }

    private void OnResume()
    {
        //animation["SpawnAnimation"].speed = 1.0f;
    }
}

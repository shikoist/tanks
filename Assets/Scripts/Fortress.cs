using UnityEngine;
using System.Collections;

public class Fortress : MonoBehaviour
{
    //public Transform explosion;
    //public Transform bigExplosion;
    private MainScript ms;

    private void Start()
    {
        ms = GameObject.Find("MainScript").GetComponent<MainScript>();
    }

    private void Update()
    {

    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (ms.gameOver == false)
        {
            string tag = otherCollider.transform.tag;
            if (tag == "Bullet" || tag == "SpecialBullet")
            {
                if (ms.cameraType != CameraType.FPS)
                {
                    ms.StartBackToMenu();
                }
                //Instantiate(bigExplosion, transform.position, Quaternion.Euler(270.0f, 90.0f, 0.0f));
                ms.bigExplosions.ShowAtPosition(transform.position);
                renderer.enabled = false;
                collider.enabled = false;
            }
        }
    }
}

// Type: BulletScript
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 34478F2D-588B-405F-8BB8-6DF8900D2758
// Assembly location: C:\Downloads\Tanks-Windows-x86-v0.917f\Tanks_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public string ally = string.Empty;
    public int number;
    public Transform explosion;
    private bool oldPause;
    private MainScript ms;
    private Vector3 oldVelocity;

    public int direction = 0;

    float timer = 0.0f;
    float timerRate = 0.1f;

    private void Start()
    {
        ms = GameObject.Find("MainScript").GetComponent<MainScript>();
    }

    private void Update()
    {
        rigidbody.MoveRotation(Quaternion.Euler(0.0f, direction * 90.0f, 0.0f));
        //rigidbody.velocity = 

        if (Network.isServer)
        {
            if (timer < Time.time)
            {
                timer = Time.time + timerRate;

                ms.nv.RPC("SetBulletPosRot", RPCMode.Others, number, transform.position, direction);
            }
        }

        if (ms.pause == oldPause)
            return;
        oldPause = ms.pause;
        if (oldPause)
            OnPause();
        else
            OnResume();
    }

    private void FixedUpdate()
    {
        if (rigidbody.position.x <= 30.0f && rigidbody.position.x >= 0.0f && (rigidbody.position.z <= 31.0f && rigidbody.position.z >= 1.0f) || !collider.enabled)
            return;
        //ms.bulletsExplosionsTransforms[number].position = transform.position;
        //ms.bulletsDetonators[number].Explode();
        ms.smallExplosions.ShowAtPosition(transform.position);
        //if (!ms.noSound)
        //{
        //    ms.bulletsExplosionsTransforms[number].audio.Play();
        //}
        rigidbody.velocity = Vector3.zero;
        renderer.enabled = false;
        collider.enabled = false;
    }

    private void OnPause()
    {
        oldVelocity = rigidbody.velocity;
        rigidbody.velocity = Vector3.zero;
    }

    private void OnResume()
    {
        rigidbody.velocity = oldVelocity;
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        string tag = otherCollider.transform.tag;
        if (!(tag == "Bricks") && !(tag == "Concrete") && !(tag == "Walls"))
            return;
        //ms.bulletsExplosionsTransforms[number].position = transform.position;
        //ms.bulletsDetonators[number].Explode();

        ms.smallExplosions.ShowAtPosition(transform.position);

        if (number < 4 || number > 23 && number < 28)
        {
            //if (!ms.noSound)
            //{
            //    ms.bulletsExplosionsTransforms[number].audio.Play();
            //}
        }
        rigidbody.velocity = Vector3.zero;
        renderer.enabled = false;
        collider.enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        for (int i = 0; i < collision.contacts.Length; i++)
        {
            if (collider.enabled)
            {
                Transform tCollider = collision.contacts[i].otherCollider.transform;
                string tag = tCollider.tag;
                if (tag == "Tank")
                {
                    string str = tCollider.GetComponent<TankScript>().ally;
                    if (str == "Player" && ally == "Enemy" || str == "Enemy" && ally == "Player")
                    {
                        rigidbody.velocity = Vector3.zero;
                        renderer.enabled = false;
                        collider.enabled = false;
                        //transform.position = Vector3.zero;
                    }
                }
                else
                {
                    //ms.bulletsExplosionsTransforms[number].position = transform.position;
                    //ms.bulletsDetonators[number].Explode();
                    //if (number < 4 || number > 23 && number < 28)
                    //{
                    //    ms.bulletsExplosionsTransforms[number].audio.Play();
                    //}
                    ms.smallExplosions.ShowAtPosition(transform.position);

                    if (!ms.noSound && !ms.audioSmallExplosion.isPlaying)
                    {
                        ms.audioSmallExplosion.Play();
                    }

                    rigidbody.velocity = Vector3.zero;
                    renderer.enabled = false;
                    collider.enabled = false;
                }
            }
        }
    }
}

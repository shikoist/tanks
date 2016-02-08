using UnityEngine;
using System.Collections;

public enum AnimType
{ 
    Once,
    Loop,
    PingPong
}

class AnimatedTiledTexture : MonoBehaviour
{
    public int columns = 2;
    public int rows = 2;
    public int maxFrame = 1;
    public float framesPerSecond = 10f;

    public int countLoop = 0;
    public int loops = 1;

    public AnimType animType;

    //the current frame to display
    public int index = 0;

    public bool backward = false;

    private float timer = 0.0f;
    private float timerRate = 1.0f;

    private Transform cameraTransform;

    //public bool isPlaying = false;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        //set the tile size of the texture (in UV units), based on the rows and columns
        Vector2 size = new Vector2(1f / columns, 1f / rows);
        renderer.sharedMaterial.SetTextureScale("_MainTex", size);
        backward = true;
        timerRate = 1.0f / framesPerSecond;
        if (maxFrame > rows * columns)
        {
            maxFrame = rows * columns;
        }
        index = -1;
    }

    void Update()
    {
        transform.rotation = cameraTransform.rotation * Quaternion.Euler(180.0f, 0.0f, 0.0f);

        timerRate = 1.0f / framesPerSecond;
        if (timer < Time.time)
        {
            timer = Time.time + timerRate;

            //move to the next index
            if (animType == AnimType.Once || animType == AnimType.Loop)
            {
                index++;
                if (index > maxFrame)
                {
                    countLoop++;
                    index = 0;
                }
            }
            else if (animType == AnimType.PingPong)
            {
                if (!backward)
                {
                    index++;
                    if (index > maxFrame)
                    {
                        countLoop++;
                        //index = 0;
                        index = maxFrame - 1;
                        backward = !backward;
                    }
                }
                else
                {
                    index--;
                    if (index < 0)
                    {
                        countLoop++;
                        //index = maxFrame;
                        index = 1;
                        backward = !backward;
                    }
                }
            }

            if (countLoop >= loops)
            {
                gameObject.SetActive(false);
                //this.enabled = false;
                //this.renderer.enabled = false;
            }

            //split into x and y indexes
            Vector2 offset = new Vector2((float)index / columns - (index / columns), //x index
                                          (index / columns) / (float)rows);          //y index

            renderer.material.SetTextureOffset("_MainTex", offset);
        }
    }

    void OnEnable()
    {
        //this.renderer.enabled = true;
        backward = false;
        index = -1;
        countLoop = 0;
    }
}
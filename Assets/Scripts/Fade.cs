using System;
using UnityEngine;

public enum FadeState
{
    ToFade,
    FromFade,
    Nothing,
}

public class Fade : MonoBehaviour
{
    public float speed = 2.0f;
    public Texture blackTexture;
    public Material blackMat;
    public FadeState fadeState;
    public float colorAlpha;
    //private FadeState oldFadeState;

    void Start()
    {
        colorAlpha = 1.0f;
        //oldFadeState = fadeState;
    }

    void Update()
    {
        //if (oldFadeState != fadeState)
        //{
        //    OnChangeFadeState();
        //    oldFadeState = fadeState;
        //}
        
        float deltaTime = Time.deltaTime;
        
        if (fadeState == FadeState.FromFade)
        {
            colorAlpha -= speed * deltaTime;
            if (colorAlpha <= 0.0f)
            {
                colorAlpha = 0.0f;
                fadeState = FadeState.Nothing;
            }
            blackMat.color = new Color(1.0f, 1.0f, 1.0f, colorAlpha);
        }
        else if (fadeState == FadeState.ToFade)
        {
            colorAlpha += speed * deltaTime;
            if (colorAlpha >= 1.0f)
            {
                colorAlpha = 1.0f;
                fadeState = FadeState.Nothing;
            }
            blackMat.color = new Color(1.0f, 1.0f, 1.0f, colorAlpha);
        }
    }

    //void OnChangeFadeState()
    //{
    //    if (oldFadeState == FadeState.Nothing && fadeState == FadeState.ToFade)
    //    {
    //        colorAlpha = 1.0f;
    //    }
    //    if (oldFadeState == FadeState.Nothing && fadeState == FadeState.FromFade)
    //    {
    //        colorAlpha = 0.0f;
    //    }
    //}

    void OnGUI()
    {
        if (Event.current.type == EventType.Repaint)
        {
            Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), blackTexture, new Rect(0, 0, 31, 31), 0, 0, 0, 0, Color.white, blackMat);
        }
    }
}

// Type: AnimTexture
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 34478F2D-588B-405F-8BB8-6DF8900D2758
// Assembly location: C:\Downloads\Tanks-Windows-x86-v0.917f\Tanks_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public enum AnimTextType
{
    Loop,
    PingPong,
    Once,
}

public class AnimTexture : MonoBehaviour
{
    public int uvAnimationTileX = 4;
    public int uvAnimationTileY = 4;
    public float animRate = 0.03f;
    public AnimTextType att;
    public bool play;
    public int startFrame;
    public int endFrame;
    private int inc;
    private int index;
    private float animTime;
    private Renderer r;

    private void Start()
    {
        r = renderer;
        inc = 1;
    }

    private void Update()
    {
        if (Time.time <= animTime)
            return;
        animTime = Time.time + animRate;
        index = index % (uvAnimationTileX * uvAnimationTileY);
        Vector2 scale = new Vector2(1f / (float)uvAnimationTileX, 1f / (float)uvAnimationTileY);
        float num1 = (float)(index % uvAnimationTileX);
        float num2 = (float)(index / uvAnimationTileX);
        renderer.material.SetTextureOffset("_MainTex", new Vector2(num1 * scale.x, (float)(1.0 - scale.y - num2 * scale.y)));
        renderer.material.SetTextureScale("_MainTex", scale);
        if (att == AnimTextType.PingPong)
        {
            if (play)
                index += inc;
            if (index > endFrame)
            {
                inc = -inc;
                index = endFrame - 1;
            }
            if (index > startFrame)
                return;
            inc = -inc;
            index = startFrame;
        }
        else if (att == AnimTextType.Loop)
        {
            if (play)
                ++index;
            if (index <= endFrame)
                return;
            index = startFrame;
        }
        else
        {
            if (att != AnimTextType.Once)
                return;
            if (play)
                ++index;
            if (index <= endFrame)
                return;
            index = startFrame;
            play = false;
            r.enabled = false;
        }
    }
}

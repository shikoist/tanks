// Type: PlayerInput
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 34478F2D-588B-405F-8BB8-6DF8900D2758
// Assembly location: C:\Downloads\Tanks-Windows-x86-v0.917f\Tanks_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private float deadZone = 0.25f;
    private int[] Y = new int[4];
    private int[] X = new int[4];
    private bool[] Fire1 = new bool[4];
    private bool[] Fire2 = new bool[4];
    private int[] oldY = new int[4];
    private int[] oldX = new int[4];
    private bool[] oldFire1 = new bool[4];
    private bool[] oldFire2 = new bool[4];
    public bool remote;
    public int number;
    private MainScript ms;
    private TankScript ts;

    private void Start()
    {
        ms = GameObject.Find("MainScript").GetComponent<MainScript>();
        ts = GetComponent<TankScript>();
    }

    private void Update()
    {
        if (collider.enabled)
        {
            if (remote)
                return;
            Fire1[number - 1] = Input.GetButton("P" + number.ToString() + "Fire1");
            Fire2[number - 1] = Input.GetButton("P" + number.ToString() + "Fire2");
            X[number - 1] = Input.GetAxis("P" + number.ToString() + "Horizontal") <= deadZone ? (Input.GetAxis("P" + number.ToString() + "Horizontal") >= -deadZone ? 0 : -1) : 1;
            Y[number - 1] = Input.GetAxis("P" + number.ToString() + "Vertical") <= deadZone ? (Input.GetAxis("P" + number.ToString() + "Vertical") >= -deadZone ? 0 : -1) : 1;
            if (X[number - 1] != oldX[number - 1])
            {
                oldX[number - 1] = ts.inputX;
                ts.inputX = X[number - 1];
                ms.OnChangeInput(transform.position, number - 1, X[number - 1], Y[number - 1], Fire1[number - 1], Fire2[number - 1]);
                ChangeSound();
            }
            if (Y[number - 1] != oldY[number - 1])
            {
                oldY[number - 1] = ts.inputY;
                ts.inputY = Y[number - 1];
                ms.OnChangeInput(transform.position, number - 1, X[number - 1], Y[number - 1], Fire1[number - 1], Fire2[number - 1]);
                ChangeSound();
            }
            if (Fire1[number - 1] != oldFire1[number - 1])
            {
                oldFire1[number - 1] = ts.fireInput;
                ts.fireInput = Fire1[number - 1];
                ms.OnChangeInput(transform.position, number - 1, X[number - 1], Y[number - 1], Fire1[number - 1], Fire2[number - 1]);
            }
            if (Fire2[number - 1] == oldFire2[number - 1])
                return;
            oldFire2[number - 1] = ts.fire2Input;
            ts.fire2Input = Fire2[number - 1];
            ms.OnChangeInput(transform.position, number - 1, X[number - 1], Y[number - 1], Fire1[number - 1], Fire2[number - 1]);
        }
    }

    private void ChangeSound()
    {
        if (ts.inputX != 0 || ts.inputY != 0)
        {
            audio.Stop();
            audio.clip = ms.soundsMotors[0];
            if (!ms.noSound)
            {
                audio.Play();
            }
        }
        else
        {
            audio.Stop();
            audio.clip = ms.soundsMotors[1];
            if (!ms.noSound)
            {
                audio.Play();
            }
        }
    }
}

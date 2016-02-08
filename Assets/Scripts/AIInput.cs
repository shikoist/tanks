// Type: AIInput
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 34478F2D-588B-405F-8BB8-6DF8900D2758
// Assembly location: C:\Downloads\Tanks-Windows-x86-v0.917f\Tanks_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class AIInput : MonoBehaviour
{
    private float timerRate = 0.5f;
    public float timerModeRate = 0.5f;
    public bool remote;
    public int number;
    private TankScript ts;
    private int x;
    private int y;
    private float timer;
    private MainScript ms;
    private int oldY;
    private int oldX;
    private bool oldFire;
    public Vector3 oldPosition;
    private int cellX;
    private int cellY;
    public int mode;
    public float timerMode;
    private int direction;
    public Transform fort;

    private void Start()
    {
        mode = 1;
        ms = GameObject.Find("MainScript").GetComponent<MainScript>();
        x = 0;
        y = -1;
        direction = 2;
        ts = GetComponent<TankScript>();
        oldPosition = transform.position;
        cellX = Mathf.RoundToInt(oldPosition.x + 0.5f);
        cellY = Mathf.RoundToInt(oldPosition.z + 0.5f);
    }

    private void Update()
    {
        if (!ms.tanksColliders[number].enabled)
            return;
        if (!remote)
        {
            if (Mathf.RoundToInt(transform.position.x + 0.5f) != cellX || Mathf.RoundToInt(transform.position.z + 0.5f) != cellY)
            {
                OnEnterCell();
                cellX = Mathf.RoundToInt(transform.position.x + 0.5f);
                cellY = Mathf.RoundToInt(transform.position.z + 0.5f);
            }
            if (ms.otherTime > timerMode)
            {
                timerMode = ms.otherTime + timerModeRate;
                ++mode;
                Debug.Log((object)(Time.time.ToString("F2") + " : Mode change " + mode.ToString()));
                if (mode > 2)
                    mode = 0;
            }
            if (ms.otherTime > timer)
            {
                timer = ms.otherTime + timerRate;
                if (Vector3.Distance(oldPosition, transform.position) < 0.5)
                    OnStuck();
                oldPosition = transform.position;
            }
            ts.fireInput = true;
            ts.inputX = x;
            ts.inputY = y;
        }
        if (ts.inputX != oldX)
        {
            oldX = ts.inputX;
            ms.OnChangeInput(transform.position, number, ts.inputX, ts.inputY, ts.fireInput, ts.fire2Input);
        }
        if (ts.inputY != oldY)
        {
            oldY = ts.inputY;
            ms.OnChangeInput(transform.position, number, ts.inputX, ts.inputY, ts.fireInput, ts.fire2Input);
        }
        if (ts.fireInput == oldFire)
            return;
        oldFire = ts.fireInput;
        ms.OnChangeInput(transform.position, number, ts.inputX, ts.inputY, ts.fireInput, ts.fire2Input);
    }

    private void OnStuck()
    {
        if (mode == 1)
            return;
        ChangeInput();
    }

    private void OnEnterCell()
    {
        if (mode == 0)
        {
            if (Random.Range(0, 16) != 0)
                return;
            ChangeInput();
        }
        else if (mode == 1)
        {
            if (ms.playersCount > 1.0)
            {
                if (number % 2 == 0)
                    ChooseDirectionToPlayer(0);
                else
                    ChooseDirectionToPlayer(1);
            }
            else
                ChooseDirectionToPlayer(0);
        }
        else
        {
            if (mode != 2)
                return;
            fort = GameObject.FindGameObjectWithTag("Fortress").transform;
            if (fort != null)
            {
                ChooseDirectionToFort();
            }
            else
            {
                ChangeInput();
            }
        }
    }

    private void ChooseDirectionToFort()
    {
        if (Random.Range(0, 2) == 0)
        {
            if (fort.position.x > transform.position.x)
            {
                y = 0;
                x = 1;
            }
            else
            {
                if (fort.position.x >= transform.position.x)
                    return;
                y = 0;
                x = -1;
            }
        }
        else if (fort.position.z > transform.position.z)
        {
            x = 0;
            y = 1;
        }
        else
        {
            if (fort.position.z >= transform.position.z)
                return;
            x = 0;
            y = -1;
        }
    }

    private void ChooseDirectionToPlayer(int player)
    {
        if (Random.Range(0, 2) == 0)
        {
            if (ms.tanksTransforms[player].position.x > transform.position.x)
            {
                y = 0;
                x = 1;
            }
            else
            {
                if (ms.tanksTransforms[player].position.x >= transform.position.x)
                    return;
                y = 0;
                x = -1;
            }
        }
        else if (ms.tanksTransforms[player].position.z > transform.position.z)
        {
            x = 0;
            y = 1;
        }
        else
        {
            if (ms.tanksTransforms[player].position.z >= transform.position.z)
                return;
            x = 0;
            y = -1;
        }
    }

    private void ChangeInput()
    {
        if (Random.Range(0, 2) == 0)
        {
            if (direction == 0)
            {
                direction = 1;
                x = 1;
                y = 0;
            }
            else if (direction == 1)
            {
                direction = 2;
                x = 0;
                y = -1;
            }
            else if (direction == 2)
            {
                direction = 3;
                x = -1;
                y = 0;
            }
            else
            {
                if (direction != 3)
                    return;
                direction = 0;
                x = 0;
                y = 1;
            }
        }
        else if (direction == 0)
        {
            direction = 3;
            x = -1;
            y = 0;
        }
        else if (direction == 3)
        {
            direction = 2;
            x = 0;
            y = -1;
        }
        else if (direction == 2)
        {
            direction = 1;
            x = 1;
            y = 0;
        }
        else
        {
            if (direction != 3)
                return;
            direction = 0;
            x = 0;
            y = 1;
        }
    }
}

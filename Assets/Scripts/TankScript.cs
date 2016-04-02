using UnityEngine;
using System.Collections;

public class TankScript : MonoBehaviour
{
    public int direction = 0;

    public int x = 0;
    public int y = 0;

    public string ally = string.Empty;
    [HideInInspector]
    public int hitPoints = 1;
    public float fireRate = 0.3f;
    private float specialRate = 0.3f;
    private float animateRate = 0.02f;
    public int speedTank = 1;
    [HideInInspector]
    public float standardBulletSpeed = 15f;
    [HideInInspector]
    public float specialBulletSpeed = 30f;
    [HideInInspector]
    public float bulletSpeed = 15f;
    [HideInInspector]
    public Transform bullet;
    public int number;
    [HideInInspector]
    public int type;
    [HideInInspector]
    public int inputX;
    [HideInInspector]
    public int inputY;
    [HideInInspector]
    public bool fireInput;
    [HideInInspector]
    public bool fire2Input;
    private bool oldFire2Input;
    [HideInInspector]
    public bool specialBullet;

    [HideInInspector]
    public int invincible = 0;
    private float invincibleTimer = 0.0f;
    private float invincibleRate = 1.0f;

    private bool onIce;
    private int nBullets;
    private float fireVal;
    private RaycastHit[] hits;
    public bool[] hitChecks;
    //Разница между верхними и нижними hitChecks в том, что вторые включаются по заданным условиям,
    // то есть в зависимости от типа и прокачки танка.
    public bool[] hitChecks2;
    private int curLab;
    private int oldLab;
    private MainScript ms;
    [HideInInspector]
    public bool special;
    private bool miganie;
    private float specialTimer;
    private float animateTimer;
    private bool animateFrame;
    public LayerMask layerMaskForWalking;
    public float modelWidth;
    private Vector2 trackTextureOffset;
    private int stepTexture;
    public bool waterWalking;
    public int rank;
    public GameObject shield;
    public GameObject ship;
    private float deltaValue;
    private int frame;

    float radiusTank = 0.8125f;
    int qualityCheck = 11;

    Transform t;

    float timer = 0.0f;
    float timerRate = 0.1f;

    Vector3 minBounds = new Vector3(2, 0, 3);
    Vector3 maxBounds = new Vector3(28, 0, 29);


    bool pressedInputX = false;
    //-----------------------------------------------------------------------------------------------------------------

    private void Start()
    {
        //rigidbody.
        t = transform;
        onIce = false;
        ms = GameObject.Find("MainScript").GetComponent<MainScript>();
        fireVal = ms.otherTime;

        hits = new RaycastHit[qualityCheck];
        hitChecks = new bool[qualityCheck];
        hitChecks2 = new bool[qualityCheck];
    }

    private void Update()
    {
        //RefreshHits();
        if (inputX == 0.0f && ms.cameraType == CameraType.FPS) pressedInputX = false;

        t.rotation = Quaternion.Euler(0.0f, direction * 90.0f, 0.0f);
        //rigidbody.MoveRotation(Quaternion.Euler(0.0f, direction * 90.0f, 0.0f));

        if (ms.tanksColliders[number].enabled && !audio.isPlaying && (inputX != 0 || inputY != 0))
        {
            if (audio.enabled && !ms.noSound)
            {
                audio.Play();
            }
        }
        else if (!ms.tanksColliders[number].enabled && audio.isPlaying && audio.enabled)
        {
            audio.Stop();
        }

        //if (ms.gameScreen == GameScreen.Game && !ms.pause (!ms.tanksColliders[number].enabled || ms.gameOver) || (ms.freezePlayers <= 0 || !(ally == "Player")) && (ms.freeze != 0 || !(ally == "Enemy")))
        if (ms.gameScreen == GameScreen.Game && !ms.pause && ms.tanksColliders[number].enabled && !ms.gameOver
            && ((ms.freezePlayers <= 0.0f && ally == "Player") || (ms.freezeEnemies <= 0.0f && ally == "Enemy")))
        {
            //CheckSpace2(t.position);
            //Debug.Break();
            if (collider.enabled)
            {
                BeyondBounds();
            }

            //Обработка времени неуязвииости
            if (invincible > 0)
            {
                if (invincibleTimer < ms.otherTime)
                {
                    invincibleTimer = ms.otherTime + invincibleRate;

                    invincible--;

                    if (invincible == 0)
                    {
                        shield.SetActive(false);
                    }
                }
            }

            //Мигание танка
            if (special && Time.time > specialTimer)
            {
                specialTimer = Time.time + specialRate;
                if (miganie)
                {
                    miganie = false;
                    ms.tanksRenderers[number].materials[0].color = Color.red;
                    ms.tanksRenderers[number].materials[1].color = Color.red;
                    ms.tanksRenderers[number].materials[2].color = Color.red;
                }
                else
                {
                    miganie = true;
                    if (type == 3)
                    {
                        ms.SetColorByHitPoints(number, hitPoints);
                    }
                    else
                    {
                        ms.tanksRenderers[number].materials[0].color = Color.grey;
                        ms.tanksRenderers[number].materials[1].color = Color.grey;
                        ms.tanksRenderers[number].materials[2].color = Color.grey;
                    }
                }
            }

            //Тип тайла, в котором сейчас находится танк.
            //curLab = ms.GetLabyrinth(transform.position + new Vector3(1.0f, 0.0f, 1.0f));
            //if (curLab != oldLab)
            //{
            //    oldLab = curLab;
            //    OnChangeLab();
            //}

            //CheckSpace(Vector3.forward);

            //Вариант с плавным стрейфом от центра танка
            //AlignCorner();

            //Вариант с плавным стрейфом от краёв танка
            if (inputX == 0.0f || inputY == 0.0f)
            {
                AlignCorner3();
                //rigidbody.velocity = Vector3.zero;
            }
            //AlignCorner3();

            //Вариант с огибанием углов, как в оригинале
            //AlignCorner2();

            if (!CheckSpace() &&
                ((inputX != 0 || inputY != 0)
                && !onIce
                || onIce))
            {
                //AlignCorner3();

                if (Time.time > animateTimer)
                {
                    animateTimer = Time.time + animateRate;
                    stepTexture++;
                    if (stepTexture > 3)
                    {
                        stepTexture = 0;
                    }
                    trackTextureOffset = new Vector2(0.0f, (float)stepTexture * 0.5f);
                    ms.tanksRenderers[number].materials[1].SetTextureOffset("_MainTex", trackTextureOffset);
                }

                //Вариант с плавным перемещением
                //number > 3 это враги.
                if (ms.cameraType == CameraType.Classic || number > 3)
                {
                    transform.Translate(Vector3.forward * speedTank * 2.0f * Time.deltaTime);
                }
                else if (ms.cameraType == CameraType.Isometric)
                {
                    transform.Translate(Vector3.forward * speedTank * 2.0f * Time.deltaTime);
                }
                else if (ms.cameraType == CameraType.FPS)
                {
                    if (inputY > 0.0f)
                    {
                        transform.Translate(Vector3.forward * speedTank * 2.0f * Time.deltaTime);
                    }
                    else if (inputY < 0.0f)
                    {
                        transform.Translate(-Vector3.forward * speedTank * 2.0f * Time.deltaTime);
                    }
                }
                //rigidbody.velocity = t.forward * speedTank * 2.0f;
                //if (rigidbody.GetRelativePointVelocity(t.forward).magnitude < 10.0f)
                //if (rigidbody.velocity.magnitude < 10.0f)
                //{
                //    rigidbody.AddForce(t.forward * 2.0f);
                //}

                //Вариант с покадровым перемещением
                //deltaValue += Time.deltaTime;
                //if (deltaValue > 0.0166f)
                //{
                //    deltaValue -= 0.0166f;
                //    frame++;
                //    if (frame > 4)
                //    {
                //        frame = 1;
                //    }
                //    if (frame <= speedTank * 2.0f)
                //    {
                //        //transform.position = new Vector3(x * 0.125f, 0, y * 0.125f);
                //        transform.Translate(Vector3.forward * 0.125f);

                //        //if (inputX > 0)
                //        //{
                //        //    x++;
                //        //}
                //        //if (inputX < 0)
                //        //{
                //        //    x--;
                //        //}
                //        //if (inputY > 0)
                //        //{
                //        //    y++;
                //        //}
                //        //if (inputY < 0)
                //        //{
                //        //    y--;
                //        //}
                //    }
                //}
            }
            else
            {
                //rigidbody.velocity = Vector3.zero;
            }
            if (!onIce)
            {
                if ((inputX != 0.0f || inputY != 0.0f) && curLab == 7)
                {
                    onIce = true;
                    Invoke("StopOnIce", 1f);
                }

                //number > 3 this is enemies.
                if (ms.cameraType == CameraType.Classic || number > 3)
                {
                    if (inputX > 0)
                    {
                        //transform.rotation = Quaternion.Euler(0.0f, 90f, 0.0f);
                        direction = 1;
                    }
                    if (inputX < 0)
                    {
                        //transform.rotation = Quaternion.Euler(0.0f, 270f, 0.0f);
                        direction = 3;
                    }
                    if (inputY > 0)
                    {
                        //transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                        direction = 0;
                    }
                    if (inputY < 0)
                    {
                        //transform.rotation = Quaternion.Euler(0.0f, 180f, 0.0f);
                        direction = 2;
                    }
                }
                else if (ms.cameraType == CameraType.Isometric)
                {
                    if (inputX > 0)
                    {
                        //transform.rotation = Quaternion.Euler(0.0f, 90f, 0.0f);
                        direction = 1;
                    }
                    if (inputX < 0)
                    {
                        //transform.rotation = Quaternion.Euler(0.0f, 270f, 0.0f);
                        direction = 3;
                    }
                    if (inputY > 0)
                    {
                        //transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                        direction = 0;
                    }
                    if (inputY < 0)
                    {
                        //transform.rotation = Quaternion.Euler(0.0f, 180f, 0.0f);
                        direction = 2;
                    }
                }
                else if (ms.cameraType == CameraType.FPS)
                {
                    if (inputX > 0 && pressedInputX == false)
                    {
                        pressedInputX = true;
                        direction++;
                        if (direction > 3)
                        {
                            direction = 0;
                        }
                    }
                    if (inputX < 0 && pressedInputX == false)
                    {
                        pressedInputX = true;
                        direction--;
                        if (direction < 0)
                        {
                            direction = 3;
                        }
                    }
                }
            }
            if (fire2Input != oldFire2Input)
            {
                oldFire2Input = fire2Input;
                if (fire2Input && !ms.bulletTransforms[number].collider.enabled)
                {
                    nBullets++;
                    fireVal = ms.otherTime + fireRate;
                    bulletSpeed = !(ally == "Player") || rank <= 0 ? standardBulletSpeed : specialBulletSpeed;
                    Fire(false);
                }
            }
            if ((fireInput && ms.otherTime > fireVal) || (fire2Input && ms.otherTime > fireVal))
            {
                if (!ms.bulletTransforms[number].collider.enabled)
                {
                    nBullets++;
                    fireVal = ms.otherTime + fireRate;
                    bulletSpeed = !(ally == "Player") || rank <= 0 ? standardBulletSpeed : specialBulletSpeed;
                    Fire(false);
                }
                else
                {
                    if (rank > 2 && ms.bulletTransforms[number + ms.allTanks].collider.enabled)
                    {
                        nBullets++;
                        fireVal = ms.otherTime + fireRate;
                        bulletSpeed = !(ally == "Player") || rank <= 0 ? standardBulletSpeed : specialBulletSpeed;
                        Fire(true);
                    }
                }
            }

            if (Network.isServer)
            {
                if (timer < Time.time)
                {
                    timer = Time.time + timerRate;

                    ms.nv.RPC("SetTankPosRot", RPCMode.Others, number, t.position, direction);
                }
            }
        }
    }

    private void OnGUI()
    {
        //for (int i = 0; i < hitChecks2.Length; i++)
        //{
        //    GUI.Label(new Rect(16, 16 * i, 1024, 32), "hitChecks2[" + i.ToString() + "]=" + hitChecks2[i].ToString());
        //}
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.GetComponent<BulletScript>() != null)
        {
            BulletScript bullet = otherCollider.GetComponent<BulletScript>();
            string str = bullet.ally;
            int otherTank = bullet.number;
            if ((str == "Player" && ally == "Enemy") || (str == "Enemy" && ally == "Player"))
            {
                otherCollider.gameObject.rigidbody.velocity = Vector3.zero;
                otherCollider.gameObject.renderer.enabled = false;
                otherCollider.gameObject.collider.enabled = false;

                //renderer.enabled = false;
                //collider.enabled = false;

                //ms.ProcessCollision(number, otherTank);
                ms.nv.RPC("ProcessCollision", RPCMode.All, number, otherTank);

                //if (ms.gameOver)
                //    return;
            }
            //if (Network.isServer)
            //{
            //    NetworkView networkView = ms.nv;
            //    string name = "ProcessCollision";
            //    int num = 2;
            //    object[] objArray = new object[2];
            //    int index1 = 0;
            //    // ISSUE: variable of a boxed type
            //    __Boxed<int> local1 = (ValueType)number;
            //    objArray[index1] = (object)local1;
            //    int index2 = 1;
            //    // ISSUE: variable of a boxed type
            //    __Boxed<int> local2 = (ValueType)otherTank;
            //    objArray[index2] = (object)local2;
            //    networkView.RPC(name, (RPCMode)num, objArray);
            //}
            //else
            //{
            //    if (Network.isClient || Network.isServer)
            //        return;
            //    ms.ProcessCollision(number, otherTank);
            //}
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        for (int i = 0; i < collision.contacts.Length; i++)
        {
            Collider otherCollider = collision.contacts[i].otherCollider;
            if (otherCollider.GetComponent<BulletScript>() != null)
            {
                BulletScript bullet = otherCollider.GetComponent<BulletScript>();
                string str = bullet.ally;
                int otherTank = bullet.number;
                if ((str == "Player" && ally == "Enemy") || (str == "Enemy" && ally == "Player"))
                {
                    otherCollider.gameObject.rigidbody.velocity = Vector3.zero;
                    otherCollider.gameObject.renderer.enabled = false;
                    otherCollider.gameObject.collider.enabled = false;

                    ms.nv.RPC("ProcessCollision", RPCMode.All, number, otherTank);
                    break;
                }
            }
        }
    }

    //-----------------------------------------------------------------------------------------------------------------

    void BeyondBounds()
    {
        if (t.localPosition.x > maxBounds.x ||
            t.localPosition.x < minBounds.x ||
            t.localPosition.z > maxBounds.z ||
            t.localPosition.z < minBounds.z)
        {
            Debug.Log(Time.time.ToString("F2") + " : BeyondBounds(); number = " + number.ToString() + ";");

            if (number >= 4)
            {
                ms.nv.RPC("ProcessCollision", RPCMode.All, number, 0);
            }
            else
            {
                ms.nv.RPC("ProcessCollision", RPCMode.All, number, 4);
            }
        }
    }

    void RefreshHits()
    {

    }

    private void AlignCorner()
    {
        hitChecks[3] = Physics.Raycast(
            transform.position - Vector3.up * 0.83f,
            transform.TransformDirection(Vector3.right),
            out hits[3],
            0.82f,
            layerMaskForWalking);
        hitChecks[4] = Physics.Raycast(
            transform.position - Vector3.up * 0.83f,
            transform.TransformDirection(Vector3.left),
            out hits[4],
            0.82f,
            layerMaskForWalking);
        if ((!(hits[0].transform != null) || !(hits[0].transform.tag != "Tank")) && (!(hits[1].transform != null) || !(hits[1].transform.tag != "Tank"))
            && (!(hits[2].transform != null) || !(hits[2].transform.tag != "Tank")))
            return;
        if (hitChecks[1] && !hitChecks[0] && (!hitChecks[2] && hits[1].transform.tag != "Walls") && (!hitChecks[3] && !hitChecks[4]) || hitChecks[1] && hitChecks[0]
            && (!hitChecks[2] && hits[1].transform.tag != "Walls") && (hits[0].transform.tag != "Walls" && !hitChecks[3] && !hitChecks[4]))
        {
            if (inputX > 0)
                transform.Translate(Vector3.left * (float)speedTank * Time.deltaTime);
            else if (inputX < 0)
                transform.Translate(Vector3.left * (float)speedTank * Time.deltaTime);
            else if (inputY > 0)
            {
                transform.Translate(Vector3.left * (float)speedTank * Time.deltaTime);
            }
            else
            {
                if (inputY >= 0)
                    return;
                transform.Translate(Vector3.left * (float)speedTank * Time.deltaTime);
            }
        }
        else
        {
            if ((hitChecks[1]
                || hitChecks[0]
                || (!hitChecks[2]
                || !(hits[2].transform.tag != "Walls"))
                || (hitChecks[3]
                || hitChecks[4]))
                && (hitChecks[1]
                || !hitChecks[0]
                || (!hitChecks[2]
                || !(hits[0].transform.tag != "Walls"))
                || (!(hits[2].transform.tag != "Walls")
                || hitChecks[3]
                || hitChecks[4])))
                return;
            if (inputX > 0)
                transform.Translate(Vector3.right * (float)speedTank * Time.deltaTime);
            else if (inputX < 0)
                transform.Translate(Vector3.right * (float)speedTank * Time.deltaTime);
            else if (inputY > 0)
            {
                transform.Translate(Vector3.right * (float)speedTank * Time.deltaTime);
            }
            else
            {
                if (inputY >= 0)
                    return;
                transform.Translate(Vector3.right * (float)speedTank * Time.deltaTime);
            }
        }
    }

    private void AlignCorner2()
    {
        if (hitChecks[0])
            return;
        if (hitChecks[1] && ((hits[1].transform.tag == "Walls" || hits[1].transform.tag == "Bricks" || (hits[1].transform.tag == "Concrete" || hits[1].transform.tag == "Water")
            || hits[1].transform.tag == "Fortress") && !waterWalking || (hits[1].transform.tag == "Walls" || hits[1].transform.tag == "Bricks"
            || (hits[1].transform.tag == "Concrete" || hits[1].transform.tag == "Fortress")) && waterWalking))
        {
            if (hitChecks[1] && inputY > 0.0f && AlignFloor(transform.position.x) > 2.0f && AlignFloor(transform.position.x) < 28.0f)
                transform.position = new Vector3(AlignFloor(transform.position.x), transform.position.y, transform.position.z);
            if (hitChecks[1] && inputY < 0.0f && AlignCeil(transform.position.x) > 2.0f && AlignCeil(transform.position.x) < 28.0f)
                transform.position = new Vector3(AlignCeil(transform.position.x), transform.position.y, transform.position.z);
            if (hitChecks[1] && inputX > 0.0f && AlignCeil(transform.position.z) > 3.0f && AlignCeil(transform.position.z) < 29.0f)
                transform.position = new Vector3(transform.position.x, transform.position.y, AlignCeil(transform.position.z));
            if (hitChecks[1] && inputX < 0.0f && AlignFloor(transform.position.z) > 3.0f && AlignFloor(transform.position.z) < 29.0f)
                transform.position = new Vector3(transform.position.x, transform.position.y, AlignFloor(transform.position.z));
        }
        if (!hitChecks[2] || (!(hits[2].transform.tag == "Walls") && !(hits[2].transform.tag == "Bricks") && (!(hits[2].transform.tag == "Concrete")
            && !(hits[2].transform.tag == "Water")) && !(hits[2].transform.tag == "Fortress") || waterWalking) && (!(hits[2].transform.tag == "Walls")
            && !(hits[2].transform.tag == "Bricks") && (!(hits[2].transform.tag == "Concrete") && !(hits[2].transform.tag == "Fortress")) || !waterWalking))
            return;
        if (hitChecks[2] && inputY > 0.0f && AlignCeil(transform.position.x) > 2.0f && AlignCeil(transform.position.x) < 28.0f)
            transform.position = new Vector3(AlignCeil(transform.position.x), transform.position.y, transform.position.z);
        if (hitChecks[2] && inputY < 0.0f && AlignFloor(transform.position.x) > 2.0f && AlignFloor(transform.position.x) < 28.0f)
            transform.position = new Vector3(AlignFloor(transform.position.x), transform.position.y, transform.position.z);
        if (hitChecks[2] && inputX > 0.0f && AlignFloor(transform.position.z) > 3.0f && AlignFloor(transform.position.z) < 29.0f)
            transform.position = new Vector3(transform.position.x, transform.position.y, AlignFloor(transform.position.z));
        if (!hitChecks[2] || inputX >= 0.0f || AlignCeil(transform.position.z) <= 3.0f || AlignCeil(transform.position.z) >= 29.0f)
            return;
        transform.position = new Vector3(transform.position.x, transform.position.y, AlignCeil(transform.position.z));
    }

    private void AlignCorner3()
    {
        int corner = 4;

        CheckSpace();

        if (LeftCornerCheck(corner))
        {
            t.Translate(Vector3.right * speedTank * 2.0f * ms.otherDelta);
            //if (!CheckSpace(t.TransformDirection(Vector3.left)))
            //{
            //    t.Translate(Vector3.left * speedTank * 2.0f * ms.otherDelta);
            //}
        }

        CheckSpace();

        if (RightCornerCheck(corner))
        {
            t.Translate(Vector3.left * speedTank * 2.0f * ms.otherDelta);
            //if (!CheckSpace(t.TransformDirection(Vector3.left)))
            //{
            //    t.Translate(Vector3.left * speedTank * 2.0f * ms.otherDelta);
            //}
        }
    }

    private bool LeftCornerCheck(int p)
    {
        bool flag1 = false;
        bool flag2 = false;

        int j = 0;
        int i = 0;

        for (i = 0; i < p; i++)
        {
            if (hitChecks2[i])
            {
                j++;
            }
        }

        if (j <= i && j > 0)
        {
            flag1 = true;
        }

        j = p;
        i = p;

        for (i = p; i < hitChecks2.Length; i++)
        {
            if (!hitChecks2[i])
            {
                j++;
            }
        }

        if (j == i)
        {
            flag2 = true;
        }

        if (flag1 && flag2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool RightCornerCheck(int p)
    {
        bool flag1 = false;
        bool flag2 = false;
        int i = 0;

        int j = 0;

        for (i = 0; i < hitChecks2.Length - p; i++)
        {
            if (!hitChecks2[i])
            {
                j++;
            }
        }

        if (j == i)
        {
            flag1 = true;
        }

        j = hitChecks2.Length - p;

        for (i = hitChecks2.Length - p; i < hitChecks2.Length; i++)
        {
            if (hitChecks2[i])
            {
                j++;
            }
        }

        if (j > hitChecks2.Length - p && j <= i)
        {
            flag2 = true;
        }

        if (flag1 && flag2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private float AlignFloor(float val)
    {
        float num = (float)(val / 2.0f - Mathf.Floor(val) / 2.0f);
        //return Mathf.Floor(num);
        return val;
    }

    private float AlignCeil(float val)
    {
        float num = (float)(val / 2.0f - Mathf.Ceil(val) / 2.0f);
        //return Mathf.Ceil(num);
        return val;
    }

    private void Fire(bool second)
    {
        Vector3 pos = transform.TransformPoint(Vector3.forward - Vector3.up * 0.5f);
        ms.nv.RPC("SpawnBullet", RPCMode.All, number, second, ally, bulletSpeed, pos, direction);
    }

    private void StopOnIce()
    {
        onIce = false;
    }

    private void OnChangeLab()
    {
        if (curLab != 7)
            return;
        onIce = true;
        Invoke("StopOnIce", 1f);
    }

    bool CheckSpace2(Vector3 pos)
    {
        bool flag = false;

        Vector3[] vectors = new Vector3[4] {
            Vector3.forward,
            Vector3.right,
            Vector3.back,
            Vector3.left
        };

        for (int j = 0; j < vectors.Length; j++)
        {
            for (int i = 0; i < hitChecks.Length; i++)
            {
                Vector3 par1 = t.localPosition + vectors[j] * radiusTank
                        - vectors[j] * i * 2 * (radiusTank / (hitChecks.Length - 1))
                        - Vector3.up * 0.625f;
                hitChecks[i] = Physics.Raycast(
                    par1,
                    vectors[j],
                    out hits[i],
                    radiusTank,
                    layerMaskForWalking);
                Debug.DrawRay(
                    par1,
                    vectors[j],
                    Color.white,
                    radiusTank);
                //Debug.Break();
                if (hitChecks[i]
                    && ((hits[i].transform.tag == "Walls"
                    || hits[i].transform.tag == "Bricks"
                    || (hits[i].transform.tag == "Concrete"
                    || hits[i].transform.tag == "Water")
                    || (hits[i].transform.tag == "Tank"
                    || hits[i].transform.tag == "Fortress"))
                    && !waterWalking
                    || (hits[i].transform.tag == "Walls"
                    || hits[i].transform.tag == "Bricks"
                    || (hits[i].transform.tag == "Concrete"
                    || hits[i].transform.tag == "Tank")
                    || hits[i].transform.tag == "Fortress")
                    && waterWalking))
                {
                    hitChecks2[i] = true;
                    flag = true;
                }
                else
                {
                    hitChecks2[i] = false;
                }
            }
        }
        //Debug.Break();
        return flag;
    }

    bool CheckSpace()
    {
        bool flag = false;

        //Проверяем переднее касание.
        //for (int i = 0; i < qualityCheck / 4; i++)
        for (int i = 0; i < qualityCheck; i++)
        {
            //Вектор тут должен быть направлен вправо.
            Vector3 par1 = t.TransformPoint(-Vector3.right * radiusTank
                    + Vector3.right * i * 2 * (radiusTank / (qualityCheck - 1))
                    - Vector3.up * 0.625f);

            //А тут вектор должен быть направлен вперёд.
            hitChecks[i] = Physics.Raycast(
                par1,
                t.TransformDirection(Vector3.forward),
                out hits[i],
                radiusTank,
                layerMaskForWalking);

            Color colorDbg = Color.red;
            if (i <= qualityCheck / 2)
            {
                colorDbg = Color.blue;
            }

            Debug.DrawRay(
                par1,
                t.TransformDirection(Vector3.forward),
                colorDbg,
                radiusTank);
            //Debug.Break();
            if (hitChecks[i]
                && ((hits[i].transform.tag == "Walls"
                || hits[i].transform.tag == "Bricks"
                || (hits[i].transform.tag == "Concrete"
                || hits[i].transform.tag == "Water")
                || (hits[i].transform.tag == "Tank"
                || hits[i].transform.tag == "Fortress"))
                && !waterWalking
                || (hits[i].transform.tag == "Walls"
                || hits[i].transform.tag == "Bricks"
                || (hits[i].transform.tag == "Concrete"
                || hits[i].transform.tag == "Tank")
                || hits[i].transform.tag == "Fortress")
                && waterWalking))
            {
                hitChecks2[i] = true;
                flag = true;
            }
            else
            {
                hitChecks2[i] = false;
            }
        }

        ////Проверяем правое касание.
        //for (int i = qualityCheck / 4; i < qualityCheck / 2; i++)
        //{
        //    //Вектор тут должен быть направлен вправо.
        //    Vector3 par1 = t.TransformPoint(Vector3.forward * radiusTank
        //            - Vector3.forward * i * 2 * (radiusTank / (qualityCheck - 1))
        //            - Vector3.up * 0.625f);

        //    //А тут вектор должен быть направлен вперёд.
        //    hitChecks[i] = Physics.Raycast(
        //        par1,
        //        t.TransformDirection(Vector3.forward),
        //        out hits[i],
        //        radiusTank,
        //        layerMaskForWalking);
        //    Debug.DrawRay(
        //        par1,
        //        t.TransformDirection(Vector3.forward),
        //        Color.white,
        //        radiusTank);
        //    //Debug.Break();
        //    if (hitChecks[i]
        //        && ((hits[i].transform.tag == "Walls"
        //        || hits[i].transform.tag == "Bricks"
        //        || (hits[i].transform.tag == "Concrete"
        //        || hits[i].transform.tag == "Water")
        //        || (hits[i].transform.tag == "Tank"
        //        || hits[i].transform.tag == "Fortress"))
        //        && !waterWalking
        //        || (hits[i].transform.tag == "Walls"
        //        || hits[i].transform.tag == "Bricks"
        //        || (hits[i].transform.tag == "Concrete"
        //        || hits[i].transform.tag == "Tank")
        //        || hits[i].transform.tag == "Fortress")
        //        && waterWalking))
        //    {
        //        hitChecks2[i] = true;
        //        flag = true;
        //    }
        //    else
        //    {
        //        hitChecks2[i] = false;
        //    }
        //}

        ////Проверяем левое касание.
        //for (int i = qualityCheck / 2; i < 3 * qualityCheck / 4; i++)
        //{
        //    //Вектор тут должен быть направлен вправо.
        //    Vector3 par1 = t.TransformPoint(Vector3.back * radiusTank
        //            - Vector3.back * i * 2 * (radiusTank / (qualityCheck - 1))
        //            - Vector3.up * 0.625f);

        //    //А тут вектор должен быть направлен вперёд.
        //    hitChecks[i] = Physics.Raycast(
        //        par1,
        //        t.TransformDirection(Vector3.forward),
        //        out hits[i],
        //        radiusTank,
        //        layerMaskForWalking);
        //    Debug.DrawRay(
        //        par1,
        //        t.TransformDirection(Vector3.forward),
        //        Color.white,
        //        radiusTank);
        //    //Debug.Break();
        //    if (hitChecks[i]
        //        && ((hits[i].transform.tag == "Walls"
        //        || hits[i].transform.tag == "Bricks"
        //        || (hits[i].transform.tag == "Concrete"
        //        || hits[i].transform.tag == "Water")
        //        || (hits[i].transform.tag == "Tank"
        //        || hits[i].transform.tag == "Fortress"))
        //        && !waterWalking
        //        || (hits[i].transform.tag == "Walls"
        //        || hits[i].transform.tag == "Bricks"
        //        || (hits[i].transform.tag == "Concrete"
        //        || hits[i].transform.tag == "Tank")
        //        || hits[i].transform.tag == "Fortress")
        //        && waterWalking))
        //    {
        //        hitChecks2[i] = true;
        //        flag = true;
        //    }
        //    else
        //    {
        //        hitChecks2[i] = false;
        //    }
        //}

        ////Проверяем заднее касание. На всякий случай.
        //for (int i = 3 * qualityCheck / 4; i < qualityCheck; i++)
        //{
        //    //Вектор тут должен быть направлен вправо.
        //    Vector3 par1 = t.TransformPoint(Vector3.left * radiusTank
        //            - Vector3.left * i * 2 * (radiusTank / (qualityCheck - 1))
        //            - Vector3.up * 0.625f);

        //    //А тут вектор должен быть направлен вперёд.
        //    hitChecks[i] = Physics.Raycast(
        //        par1,
        //        t.TransformDirection(Vector3.forward),
        //        out hits[i],
        //        radiusTank,
        //        layerMaskForWalking);
        //    Debug.DrawRay(
        //        par1,
        //        t.TransformDirection(Vector3.forward),
        //        Color.white,
        //        radiusTank);
        //    //Debug.Break();
        //    if (hitChecks[i]
        //        && ((hits[i].transform.tag == "Walls"
        //        || hits[i].transform.tag == "Bricks"
        //        || (hits[i].transform.tag == "Concrete"
        //        || hits[i].transform.tag == "Water")
        //        || (hits[i].transform.tag == "Tank"
        //        || hits[i].transform.tag == "Fortress"))
        //        && !waterWalking
        //        || (hits[i].transform.tag == "Walls"
        //        || hits[i].transform.tag == "Bricks"
        //        || (hits[i].transform.tag == "Concrete"
        //        || hits[i].transform.tag == "Tank")
        //        || hits[i].transform.tag == "Fortress")
        //        && waterWalking))
        //    {
        //        hitChecks2[i] = true;
        //        flag = true;
        //    }
        //    else
        //    {
        //        hitChecks2[i] = false;
        //    }
        //}

        //Debug.Break();
        return flag;
    }
    bool CheckSpace(Vector3 dir)
    {
        bool flag = false;

        Vector3 dir2 = Vector3.zero;
        if (dir == Vector3.forward)
        {
            dir2 = Vector3.right;
        }
        else if (dir == Vector3.right)
        {
            dir2 = Vector3.back;
        }
        else if (dir == Vector3.back)
        {
            dir2 = Vector3.left;
        }
        else if (dir == Vector3.left)
        {
            dir2 = Vector3.forward;
        }

        for (int i = 0; i < qualityCheck; i++)
        {
            //Вектор тут должен быть направлен вправо.
            Vector3 par1 =
                    dir2 * radiusTank
                    - Vector3.right * i * 2 * (radiusTank / (qualityCheck - 1))
                    - Vector3.up * 0.625f;

            //А тут вектор должен быть направлен вперёд.
            hitChecks[i] = Physics.Raycast(
                par1,
                dir,
                out hits[i],
                radiusTank,
                layerMaskForWalking);
            Debug.DrawRay(
                par1,
                dir,
                Color.white,
                radiusTank);
            //Debug.Break();
            if (hitChecks[i]
                && ((hits[i].transform.tag == "Walls"
                || hits[i].transform.tag == "Bricks"
                || (hits[i].transform.tag == "Concrete"
                || hits[i].transform.tag == "Water")
                || (hits[i].transform.tag == "Tank"
                || hits[i].transform.tag == "Fortress"))
                && !waterWalking
                || (hits[i].transform.tag == "Walls"
                || hits[i].transform.tag == "Bricks"
                || (hits[i].transform.tag == "Concrete"
                || hits[i].transform.tag == "Tank")
                || hits[i].transform.tag == "Fortress")
                && waterWalking))
            {
                hitChecks2[i] = true;
                flag = true;
            }
            else
            {
                hitChecks2[i] = false;
            }
        }
        //Debug.Break();
        return flag;
    }

    private bool Check()
    {
        bool flag = false;
        for (int i = 0; i < 3; ++i)
        {
            if (hitChecks[i]
                && ((hits[i].transform.tag == "Walls"
                || hits[i].transform.tag == "Bricks"
                || (hits[i].transform.tag == "Concrete"
                || hits[i].transform.tag == "Water")
                || (hits[i].transform.tag == "Tank"
                || hits[i].transform.tag == "Fortress"))
                && !waterWalking
                || (hits[i].transform.tag == "Walls"
                || hits[i].transform.tag == "Bricks"
                || (hits[i].transform.tag == "Concrete"
                || hits[i].transform.tag == "Tank")
                || hits[i].transform.tag == "Fortress")
                && waterWalking))
                return true;
        }
        return flag;
    }

    public void Upgrade(int n)
    {
        rank += n;
        if (rank > 3)
            rank = 3;
        transform.GetComponent<MeshFilter>().mesh = ms.playerMeshes[rank];
        if (rank == 0)
        {
            hitPoints = 1;
        }
        else
        {
            if (rank != 3)
                return;
            hitPoints = 4;
        }
    }

    public void SetRank(int n)
    {
        rank = n;
        if (rank > 3)
            rank = 3;
        transform.GetComponent<MeshFilter>().mesh = ms.playerMeshes[rank];
        if (rank == 0)
        {
            hitPoints = 1;
        }
        else
        {
            if (rank != 3)
                return;
            hitPoints = 2;
        }
    }

    public void SetInvincibility(int time)
    {
        invincible = time;
        shield.SetActive(true);
    }

    public void SetWaterWalking(bool w)
    {
        if (w == true)
        {
            waterWalking = true;
            hitPoints = 2;
            ship.SetActive(true);
        }
        else
        {
            waterWalking = false;
            hitPoints = 1;
            ship.SetActive(false);
        }
    }

    
}

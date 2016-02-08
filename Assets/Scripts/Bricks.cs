using UnityEngine;
using System.Collections;

public class Bricks : MonoBehaviour
{
    public int x = 0;
    public int y = 0;
    
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] UV;
    
    private MeshFilter mf;
    private MeshCollider mc;
    
    public bool[] destroyedBlocks;
    public bool[] oldDestroyedBlocks;
    
    private MainScript ms;
    private string myTag;

    //Восемь точек в блоке. Недостаток - нельзя корректно натянуть текстуру на боковые стенки.
    //Vector3[] blockVertices = new Vector3[8] 
    //{
    //    new Vector3(-0.25f, 0.0f, 0.25f), //00
    //    new Vector3( 0.25f, 0.0f, 0.25f), //01
    //    new Vector3(-0.25f,-1.0f, 0.25f), //02
    //    new Vector3( 0.25f,-1.0f, 0.25f), //03
    //    new Vector3( 0.25f, 0.0f,-0.25f), //04
    //    new Vector3( 0.25f,-1.0f,-0.25f), //05
    //    new Vector3(-0.25f, 0.0f,-0.25f), //06
    //    new Vector3(-0.25f,-1.0f,-0.25f)  //07
    //};

    //В блоке 20 точек. Текстуры натягиваются корректно.
    Vector3[] blockVertices = new Vector3[] 
    {
        //...............x,.....y,.....z
        new Vector3(-0.25f, 0.00f, 0.25f), //00
        new Vector3( 0.25f, 0.00f, 0.25f), //01
        new Vector3( 0.25f,-1.00f, 0.25f), //02
        new Vector3(-0.25f,-1.00f, 0.25f), //03

        new Vector3( 0.25f, 0.00f, 0.25f), //04
        new Vector3( 0.25f, 0.00f,-0.25f), //05
        new Vector3( 0.25f,-1.00f,-0.25f), //06
        new Vector3( 0.25f,-1.00f, 0.25f), //07

        new Vector3(-0.25f, 0.00f,-0.25f), //08
        new Vector3( 0.25f, 0.00f,-0.25f), //09
        new Vector3( 0.25f,-1.00f,-0.25f), //10
        new Vector3(-0.25f,-1.00f,-0.25f), //11

        new Vector3(-0.25f, 0.00f, 0.25f), //12
        new Vector3(-0.25f, 0.00f,-0.25f), //13
        new Vector3(-0.25f,-1.00f,-0.25f), //14
        new Vector3(-0.25f,-1.00f, 0.25f), //15

        new Vector3(-0.25f, 0.00f, 0.25f), //16
        new Vector3(-0.25f, 0.00f,-0.25f), //17
        new Vector3( 0.25f, 0.00f,-0.25f), //18
        new Vector3( 0.25f, 0.00f, 0.25f)  //19
    };
        
    //Вариант, когда один кирпич - одна текстура. Сильно мелко.
    //Vector2[] blockUV = new Vector2[8]
    //{
    //    new Vector2(0.0f, 0.0f),
    //    new Vector2(0.0f, 1.0f),
    //    new Vector2(0.0f, 0.0f),
    //    new Vector2(0.0f, 1.0f),
    //    new Vector2(1.0f, 1.0f),
    //    new Vector2(1.0f, 1.0f),
    //    new Vector2(1.0f, 0.0f),
    //    new Vector2(1.0f, 0.0f)
    //};

    //Vector2[] blockUV = new Vector2[8]
    //{
    //    new Vector2(0.0f, 0.0f),
    //    new Vector2(0.0f, 1.0f),
    //    new Vector2(0.0f, 0.0f),
    //    new Vector2(0.0f, 1.0f),
    //    new Vector2(1.0f, 1.0f),
    //    new Vector2(1.0f, 1.0f),
    //    new Vector2(1.0f, 0.0f),
    //    new Vector2(1.0f, 0.0f)
    //};

    //Старый вариант.
    //Vector2[] blockUV = new Vector2[8]
    //{
    //    //new Vector2(0.00f, 0.00f), //0
    //    new Vector2(0.00f, 0.25f), //0
    //    //new Vector2(0.00f, 0.25f), //1
    //    new Vector2(0.25f, 0.25f), //1
    //    new Vector2(0.00f, 0.25f), //2
    //    new Vector2(0.25f, 0.25f), //3
    //    //new Vector2(0.25f, 0.25f), //4
    //    new Vector2(0.25f, 0.00f), //4
    //    new Vector2(0.25f, 0.00f), //5
    //    //new Vector2(0.25f, 0.00f), //6
    //    new Vector2(0.00f, 0.00f), //6
    //    new Vector2(0.00f, 0.00f)  //7
    //};

    //Новый вариант.
    Vector2[] blockUV = new Vector2[]
    {
        new Vector2(0.25f, 0.25f), //16
        new Vector2(0.25f, 0.00f), //17
        new Vector2(0.00f, 0.00f), //18
        new Vector2(0.00f, 0.25f), //19

        new Vector2(0.25f, 0.25f), //16
        new Vector2(0.25f, 0.00f), //17
        new Vector2(0.00f, 0.00f), //18
        new Vector2(0.00f, 0.25f), //19

        new Vector2(0.25f, 0.25f), //16
        new Vector2(0.25f, 0.00f), //17
        new Vector2(0.00f, 0.00f), //18
        new Vector2(0.00f, 0.25f), //19

        new Vector2(0.25f, 0.25f), //16
        new Vector2(0.25f, 0.00f), //17
        new Vector2(0.00f, 0.00f), //18
        new Vector2(0.00f, 0.25f), //19

        new Vector2(0.00f, 0.25f), //16
        new Vector2(0.00f, 0.00f), //17
        new Vector2(0.25f, 0.00f), //18
        new Vector2(0.25f, 0.25f) //19
    };
        
    //Old.
    //int[] blockTriangles = new int[30]
    //{   
    //    2, 1, 0,
    //    1, 2, 3,
    //    3, 5, 4,
    //    4, 1, 3,
    //    7, 6, 4,
    //    5, 7, 4,
    //    2, 0, 6,
    //    7, 2, 6,
    //    1, 4, 6,
    //    6, 0, 1
    //};

    //New.
    int[] blockTriangles = new int[]
    {   
        0,  3,  2,
        0,  2,  1,
        4,  6,  5,
        4,  7,  6,
        8, 10, 11,
        8,  9, 10,
       12, 14, 15,
       12, 13, 14,
       16, 19, 18,
       16, 18, 17
    };

    void Start()
    {
        ms = GameObject.Find("MainScript").GetComponent<MainScript>();
        mc = this.GetComponent<MeshCollider>();
        mf = this.GetComponent<MeshFilter>();

        myTag = transform.tag;
        if (destroyedBlocks == null)
        {
            destroyedBlocks = new bool[16];
            for (int i = 0; i < 16; i++)
            {
                destroyedBlocks[i] = true;
            }
        }
        if (oldDestroyedBlocks == null)
        {
            oldDestroyedBlocks = new bool[16];
            for (int i = 0; i < 16; i++)
            {
                oldDestroyedBlocks[i] = true;
            }
        }
        
        RefreshBricks();
    }

    private void Update()
    {
        bool changed = false;
        for (int i = 0; i < 16; i++)
        {
            if (destroyedBlocks[i] != oldDestroyedBlocks[i])
            {
                oldDestroyedBlocks[i] = destroyedBlocks[i];
                changed = true;
            }
        }
        if (changed)
        {
            RefreshBricks();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Network.isClient)
        {
            return;
        }

        for (int i = 0; i < collision.contacts.Length; i++)
        {

            Collider otherCollider = collision.contacts[i].otherCollider;

            bool flag = false;
            string tag = otherCollider.transform.tag;
            //int num1 = 0;

            if ((tag == "Bullet" && myTag == "Bricks") || (tag == "SpecialBullet" && myTag == "Concrete"))
            {
                //int num2 = Mathf.RoundToInt(otherCollider.transform.rotation.eulerAngles.y);
                int num2 = otherCollider.gameObject.GetComponent<BulletScript>().direction * 90;
                float angle = otherCollider.transform.rotation.eulerAngles.y;
                //       45   0  315
                //       90      270
                //       135 180 225
                if (angle >= 225.0f && angle < 315.0f)
                {
                    num2 = 270;
                }
                if (angle >= 135.0f && angle < 225.0f)
                {
                    num2 = 180;
                }
                if (angle >= 45.0f && angle < 135.0f)
                {
                    num2 = 90;
                }
                if ((angle >= 315.0f && angle <= 360.0f) || (angle < 45.0f && angle >= 0.0f))
                {
                    num2 = 0;
                }
                Vector3 vector3 = transform.InverseTransformPoint(otherCollider.transform.position);
                //Debug.Log("Bullet Rotation: " + num2.ToString() + " , Bullet Position: " + vector3.ToString());
                float num3 = 0.51f;
                if (vector3.z > -num3 && vector3.z < num3 && num2 == 270)
                {
                    if (flag)
                        Debug.Log((object)"DeleteRightBlocks");
                    DeleteRightBlocks();
                }
                else if (vector3.z > num3 && num2 == 270)
                {
                    if (flag)
                        Debug.Log((object)"DeleteRightUpUpBlocks");
                    DeleteRightUpUpBlocks();
                }
                else if (vector3.z < -num3 && num2 == 270)
                {
                    if (flag)
                        Debug.Log((object)"DeleteRightDownDownBlocks");
                    DeleteRightDownDownBlocks();
                }
                else if (vector3.z > -num3 && vector3.z < num3 && num2 == 90)
                {
                    if (flag)
                        Debug.Log((object)"DeleteLeftBlocks");
                    DeleteLeftBlocks();
                }
                else if (vector3.z > num3 && num2 == 90)
                {
                    if (flag)
                        Debug.Log((object)"DeleteLeftUpUpBlocks");
                    DeleteLeftUpUpBlocks();
                }
                else if (vector3.z < -num3 && num2 == 90)
                {
                    if (flag)
                        Debug.Log((object)("relativePosition = " + vector3.ToString()));
                    if (flag)
                        Debug.Log((object)"DeleteLeftDownDownBlocks");
                    DeleteLeftDownDownBlocks();
                }
                else if (vector3.x > -num3 && vector3.x < num3 && num2 == 0)
                {
                    if (flag)
                        Debug.Log((object)"DeleteDownBlocks");
                    DeleteDownBlocks();
                }
                else if (vector3.x < -num3 && num2 == 0)
                {
                    if (flag)
                        Debug.Log((object)"DeleteDownLeftLeftBlocks");
                    DeleteDownLeftLeftBlocks();
                }
                else if (vector3.x > num3 && num2 == 0)
                {
                    if (flag)
                        Debug.Log((object)"DeleteDownRightRightBlocks");
                    DeleteDownRightRightBlocks();
                }
                else if (vector3.x > -num3 && vector3.x < num3 && num2 == 180)
                {
                    if (flag)
                        Debug.Log((object)"DeleteUpBlocks");
                    DeleteUpBlocks();
                }
                else if (vector3.x < -num3 && num2 == 180)
                {
                    if (flag)
                        Debug.Log((object)"DeleteUpLeftLeftBlocks");
                    DeleteUpLeftLeftBlocks();
                }
                else
                {
                    if (vector3.x <= num3 || num2 != 180)
                        return;
                    if (flag)
                        Debug.Log((object)"DeleteUpRightRightBlocks");
                    DeleteUpRightRightBlocks();
                }
            }
            break;
        }

        if (Network.isServer)
        {
            string strBlockState = "";
            for (int i = 0; i < 16; i++)
            {
                if (destroyedBlocks[i])
                {
                    strBlockState += '1';
                }
                else
                {
                    strBlockState += '0';
                }
            }
            ms.nv.RPC("SetBlockState", RPCMode.Others, x, y, strBlockState);
        }
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (Network.isClient)
        {
            return;
        }
        
        bool flag = false;
        string tag = otherCollider.transform.tag;
        //int num1 = 0;

        if ((tag == "Bullet" && myTag == "Bricks") || (tag == "SpecialBullet" && myTag == "Concrete"))
        {
            //int num2 = Mathf.RoundToInt(otherCollider.transform.rotation.eulerAngles.y);
            int num2 = otherCollider.gameObject.GetComponent<BulletScript>().direction * 90;
            Vector3 vector3 = transform.InverseTransformPoint(otherCollider.transform.position);
            //Debug.Log("Bullet Rotation: " + num2.ToString() + " , Bullet Position: " + vector3.ToString());
            float num3 = 0.51f;
            if (vector3.z > -num3 && vector3.z < num3 && num2 == 270)
            {
                if (flag)
                    Debug.Log((object)"DeleteRightBlocks");
                DeleteRightBlocks();
            }
            else if (vector3.z > num3 && num2 == 270)
            {
                if (flag)
                    Debug.Log((object)"DeleteRightUpUpBlocks");
                DeleteRightUpUpBlocks();
            }
            else if (vector3.z < -num3 && num2 == 270)
            {
                if (flag)
                    Debug.Log((object)"DeleteRightDownDownBlocks");
                DeleteRightDownDownBlocks();
            }
            else if (vector3.z > -num3 && vector3.z < num3 && num2 == 90)
            {
                if (flag)
                    Debug.Log((object)"DeleteLeftBlocks");
                DeleteLeftBlocks();
            }
            else if (vector3.z > num3 && num2 == 90)
            {
                if (flag)
                    Debug.Log((object)"DeleteLeftUpUpBlocks");
                DeleteLeftUpUpBlocks();
            }
            else if (vector3.z < -num3 && num2 == 90)
            {
                if (flag)
                    Debug.Log((object)("relativePosition = " + vector3.ToString()));
                if (flag)
                    Debug.Log((object)"DeleteLeftDownDownBlocks");
                DeleteLeftDownDownBlocks();
            }
            else if (vector3.x > -num3 && vector3.x < num3 && num2 == 0)
            {
                if (flag)
                    Debug.Log((object)"DeleteDownBlocks");
                DeleteDownBlocks();
            }
            else if (vector3.x < -num3 && num2 == 0)
            {
                if (flag)
                    Debug.Log((object)"DeleteDownLeftLeftBlocks");
                DeleteDownLeftLeftBlocks();
            }
            else if (vector3.x > num3 && num2 == 0)
            {
                if (flag)
                    Debug.Log((object)"DeleteDownRightRightBlocks");
                DeleteDownRightRightBlocks();
            }
            else if (vector3.x > -num3 && vector3.x < num3 && num2 == 180)
            {
                if (flag)
                    Debug.Log((object)"DeleteUpBlocks");
                DeleteUpBlocks();
            }
            else if (vector3.x < -num3 && num2 == 180)
            {
                if (flag)
                    Debug.Log((object)"DeleteUpLeftLeftBlocks");
                DeleteUpLeftLeftBlocks();
            }
            else
            {
                if (vector3.x <= num3 || num2 != 180)
                    return;
                if (flag)
                    Debug.Log((object)"DeleteUpRightRightBlocks");
                DeleteUpRightRightBlocks();
            }
        }

        if (Network.isServer)
        {
            string strBlockState = "";
            for (int i = 0; i < 16; i++)
            {
                if (destroyedBlocks[i])
                {
                    strBlockState += '1';
                }
                else
                {
                    strBlockState += '0';
                }
            }
            ms.nv.RPC("SetBlockState", RPCMode.Others, x, y, strBlockState);
        }
    }

    private void DeleteRightBlocks()
    {
        if (!destroyedBlocks[15] || !destroyedBlocks[14] || (!destroyedBlocks[13] || !destroyedBlocks[12]))
        {
            destroyedBlocks[15] = true;
            destroyedBlocks[14] = true;
            destroyedBlocks[13] = true;
            destroyedBlocks[12] = true;
        }
        else if (!destroyedBlocks[11] || !destroyedBlocks[10] || (!destroyedBlocks[9] || !destroyedBlocks[8]))
        {
            destroyedBlocks[11] = true;
            destroyedBlocks[10] = true;
            destroyedBlocks[9] = true;
            destroyedBlocks[8] = true;
        }
        else if (!destroyedBlocks[7] || !destroyedBlocks[6] || (!destroyedBlocks[5] || !destroyedBlocks[4]))
        {
            destroyedBlocks[7] = true;
            destroyedBlocks[6] = true;
            destroyedBlocks[5] = true;
            destroyedBlocks[4] = true;
        }
        else
        {
            if (destroyedBlocks[3] && destroyedBlocks[2] && (destroyedBlocks[1] && destroyedBlocks[0]))
                return;
            destroyedBlocks[3] = true;
            destroyedBlocks[2] = true;
            destroyedBlocks[1] = true;
            destroyedBlocks[0] = true;
        }
    }
    private void DeleteLeftBlocks()
    {
        if (!destroyedBlocks[3] || !destroyedBlocks[2] || (!destroyedBlocks[1] || !destroyedBlocks[0]))
        {
            destroyedBlocks[3] = true;
            destroyedBlocks[2] = true;
            destroyedBlocks[1] = true;
            destroyedBlocks[0] = true;
        }
        else if (!destroyedBlocks[7] || !destroyedBlocks[6] || (!destroyedBlocks[5] || !destroyedBlocks[4]))
        {
            destroyedBlocks[7] = true;
            destroyedBlocks[6] = true;
            destroyedBlocks[5] = true;
            destroyedBlocks[4] = true;
        }
        else if (!destroyedBlocks[11] || !destroyedBlocks[10] || (!destroyedBlocks[9] || !destroyedBlocks[8]))
        {
            destroyedBlocks[11] = true;
            destroyedBlocks[10] = true;
            destroyedBlocks[9] = true;
            destroyedBlocks[8] = true;
        }
        else
        {
            if (destroyedBlocks[15] && destroyedBlocks[14] && (destroyedBlocks[13] && destroyedBlocks[12]))
                return;
            destroyedBlocks[15] = true;
            destroyedBlocks[14] = true;
            destroyedBlocks[13] = true;
            destroyedBlocks[12] = true;
        }
    }
    private void DeleteUpBlocks()
    {
        if (!destroyedBlocks[3] || !destroyedBlocks[7] || (!destroyedBlocks[11] || !destroyedBlocks[15]))
        {
            destroyedBlocks[3] = true;
            destroyedBlocks[7] = true;
            destroyedBlocks[11] = true;
            destroyedBlocks[15] = true;
        }
        else if (!destroyedBlocks[2] || !destroyedBlocks[6] || (!destroyedBlocks[10] || !destroyedBlocks[14]))
        {
            destroyedBlocks[2] = true;
            destroyedBlocks[6] = true;
            destroyedBlocks[10] = true;
            destroyedBlocks[14] = true;
        }
        else if (!destroyedBlocks[1] || !destroyedBlocks[5] || (!destroyedBlocks[9] || !destroyedBlocks[13]))
        {
            destroyedBlocks[1] = true;
            destroyedBlocks[5] = true;
            destroyedBlocks[9] = true;
            destroyedBlocks[13] = true;
        }
        else
        {
            if (destroyedBlocks[0] && destroyedBlocks[4] && (destroyedBlocks[8] && destroyedBlocks[12]))
                return;
            destroyedBlocks[0] = true;
            destroyedBlocks[4] = true;
            destroyedBlocks[8] = true;
            destroyedBlocks[12] = true;
        }
    }
    private void DeleteDownBlocks()
    {
        if (!destroyedBlocks[0] || !destroyedBlocks[4] || (!destroyedBlocks[8] || !destroyedBlocks[12]))
        {
            destroyedBlocks[0] = true;
            destroyedBlocks[4] = true;
            destroyedBlocks[8] = true;
            destroyedBlocks[12] = true;
        }
        else if (!destroyedBlocks[1] || !destroyedBlocks[5] || (!destroyedBlocks[9] || !destroyedBlocks[13]))
        {
            destroyedBlocks[1] = true;
            destroyedBlocks[5] = true;
            destroyedBlocks[9] = true;
            destroyedBlocks[13] = true;
        }
        else if (!destroyedBlocks[2] || !destroyedBlocks[6] || (!destroyedBlocks[10] || !destroyedBlocks[14]))
        {
            destroyedBlocks[2] = true;
            destroyedBlocks[6] = true;
            destroyedBlocks[10] = true;
            destroyedBlocks[14] = true;
        }
        else
        {
            if (destroyedBlocks[3] && destroyedBlocks[7] && (destroyedBlocks[11] && destroyedBlocks[15]))
                return;
            destroyedBlocks[3] = true;
            destroyedBlocks[7] = true;
            destroyedBlocks[11] = true;
            destroyedBlocks[15] = true;
        }
    }
    private void DeleteLeftUpUpBlocks()
    {
        if (!destroyedBlocks[3] || !destroyedBlocks[2])
        {
            destroyedBlocks[3] = true;
            destroyedBlocks[2] = true;
        }
        else if (!destroyedBlocks[7] || !destroyedBlocks[6])
        {
            destroyedBlocks[7] = true;
            destroyedBlocks[6] = true;
        }
        else if (!destroyedBlocks[11] || !destroyedBlocks[10])
        {
            destroyedBlocks[11] = true;
            destroyedBlocks[10] = true;
        }
        else
        {
            if (destroyedBlocks[15] && destroyedBlocks[14])
                return;
            destroyedBlocks[15] = true;
            destroyedBlocks[14] = true;
        }
    }
    private void DeleteLeftDownDownBlocks()
    {
        if (!destroyedBlocks[1] || !destroyedBlocks[0])
        {
            destroyedBlocks[1] = true;
            destroyedBlocks[0] = true;
        }
        else if (!destroyedBlocks[5] || !destroyedBlocks[4])
        {
            destroyedBlocks[5] = true;
            destroyedBlocks[4] = true;
        }
        else if (!destroyedBlocks[9] || !destroyedBlocks[8])
        {
            destroyedBlocks[9] = true;
            destroyedBlocks[8] = true;
        }
        else
        {
            if (destroyedBlocks[13] && destroyedBlocks[12])
                return;
            destroyedBlocks[13] = true;
            destroyedBlocks[12] = true;
        }
    }
    private void DeleteRightUpUpBlocks()
    {
        if (!destroyedBlocks[15] || !destroyedBlocks[14])
        {
            destroyedBlocks[15] = true;
            destroyedBlocks[14] = true;
        }
        else if (!destroyedBlocks[11] || !destroyedBlocks[10])
        {
            destroyedBlocks[11] = true;
            destroyedBlocks[10] = true;
        }
        else if (!destroyedBlocks[7] || !destroyedBlocks[6])
        {
            destroyedBlocks[7] = true;
            destroyedBlocks[6] = true;
        }
        else
        {
            if (destroyedBlocks[3] && destroyedBlocks[2])
                return;
            destroyedBlocks[3] = true;
            destroyedBlocks[2] = true;
        }
    }
    private void DeleteRightDownDownBlocks()
    {
        if (!destroyedBlocks[13] || !destroyedBlocks[12])
        {
            destroyedBlocks[13] = true;
            destroyedBlocks[12] = true;
        }
        else if (!destroyedBlocks[9] || !destroyedBlocks[8])
        {
            destroyedBlocks[9] = true;
            destroyedBlocks[8] = true;
        }
        else if (!destroyedBlocks[5] || !destroyedBlocks[4])
        {
            destroyedBlocks[5] = true;
            destroyedBlocks[4] = true;
        }
        else
        {
            if (!destroyedBlocks[1] && destroyedBlocks[0])
                return;
            destroyedBlocks[1] = true;
            destroyedBlocks[0] = true;
        }
    }
    private void DeleteUpLeftLeftBlocks()
    {
        if (!destroyedBlocks[3] || !destroyedBlocks[7])
        {
            destroyedBlocks[3] = true;
            destroyedBlocks[7] = true;
        }
        else if (!destroyedBlocks[2] || !destroyedBlocks[6])
        {
            destroyedBlocks[2] = true;
            destroyedBlocks[6] = true;
        }
        else if (!destroyedBlocks[1] || !destroyedBlocks[5])
        {
            destroyedBlocks[1] = true;
            destroyedBlocks[5] = true;
        }
        else
        {
            if (destroyedBlocks[0] && destroyedBlocks[4])
                return;
            destroyedBlocks[0] = true;
            destroyedBlocks[4] = true;
        }
    }
    private void DeleteUpRightRightBlocks()
    {
        if (!destroyedBlocks[11] || !destroyedBlocks[15])
        {
            destroyedBlocks[11] = true;
            destroyedBlocks[15] = true;
        }
        else if (!destroyedBlocks[10] || !destroyedBlocks[14])
        {
            destroyedBlocks[10] = true;
            destroyedBlocks[14] = true;
        }
        else if (!destroyedBlocks[9] || !destroyedBlocks[13])
        {
            destroyedBlocks[9] = true;
            destroyedBlocks[13] = true;
        }
        else
        {
            if (destroyedBlocks[8] && destroyedBlocks[12])
                return;
            destroyedBlocks[8] = true;
            destroyedBlocks[12] = true;
        }
    }
    private void DeleteDownLeftLeftBlocks()
    {
        if (!destroyedBlocks[0] || !destroyedBlocks[4])
        {
            destroyedBlocks[0] = true;
            destroyedBlocks[4] = true;
        }
        else if (!destroyedBlocks[1] || !destroyedBlocks[5])
        {
            destroyedBlocks[1] = true;
            destroyedBlocks[5] = true;
        }
        else if (!destroyedBlocks[2] || !destroyedBlocks[6])
        {
            destroyedBlocks[2] = true;
            destroyedBlocks[6] = true;
        }
        else
        {
            if (destroyedBlocks[3] && destroyedBlocks[7])
                return;
            destroyedBlocks[3] = true;
            destroyedBlocks[7] = true;
        }
    }
    private void DeleteDownRightRightBlocks()
    {
        if (!destroyedBlocks[8] || !destroyedBlocks[12])
        {
            destroyedBlocks[8] = true;
            destroyedBlocks[12] = true;
        }
        else if (!destroyedBlocks[9] || !destroyedBlocks[13])
        {
            destroyedBlocks[9] = true;
            destroyedBlocks[13] = true;
        }
        else if (!destroyedBlocks[10] || !destroyedBlocks[14])
        {
            destroyedBlocks[10] = true;
            destroyedBlocks[14] = true;
        }
        else
        {
            if (destroyedBlocks[11] && destroyedBlocks[15])
                return;
            destroyedBlocks[11] = true;
            destroyedBlocks[15] = true;
        }
    }

    void RefreshBricks()
    {
        int numberBlocks = 16;
        int count = 0;
        Mesh mesh = new Mesh();
        for (int i = 0; i < numberBlocks; i++)
        {
            if (destroyedBlocks[i])
            {
                count++;
            }
        }
        int length2 = (numberBlocks - count) * blockVertices.Length;
        int length3 = (numberBlocks - count) * blockTriangles.Length;

        vertices = new Vector3[length2];
        UV = new Vector2[length2];
        triangles = new int[length3];

        int a = 0;
        int b = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (!destroyedBlocks[b])
                {
                    for (int k = 0; k < blockVertices.Length; k++)
                    {
                        vertices[a] = blockVertices[k] + new Vector3(
                            (float)(0.5f * i - 0.75f),
                            0.0f,
                            (float)(0.5f * j - 0.75f));
                        UV[a] = blockUV[k] + new Vector2(0.25f * i, 0.25f * j);
                        a++;
                    }
                }
                b++;
            }
        }

        int c = 0;
        int d = 0;
        int num = 0;
        for (int i = 0; i < 4; ++i)
        {
            for (int j = 0; j < 4; ++j)
            {
                if (!destroyedBlocks[d])
                {
                    for (int k = 0; k < blockTriangles.Length; ++k)
                    {
                        triangles[c] = blockTriangles[k] + blockVertices.Length * num;
                        ++c;
                    }
                    ++num;
                }
                ++d;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = UV;

        mesh.RecalculateNormals();
        
        mf.mesh = mesh;

        mc.sharedMesh = null;
        mc.sharedMesh = mesh;
        //mc.isTrigger = true;
    }
}

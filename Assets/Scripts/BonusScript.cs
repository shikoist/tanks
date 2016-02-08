// Type: BonusScript
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 34478F2D-588B-405F-8BB8-6DF8900D2758
// Assembly location: C:\Downloads\Tanks-Windows-x86-v0.917f\Tanks_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

public class BonusScript : MonoBehaviour
{
    private float bonusRate = 0.25f;
    public Bonus type;
    public MainScript ms;
    private float bonusTimer;
    private MeshRenderer mr;
    private bool miganie;
    private float destroyTimer;
    private float destroyRate;
    public Transform shield;
    public Transform ship;

    private void Start()
    {
        ms = GameObject.Find("MainScript").GetComponent<MainScript>();
        mr = transform.GetComponent<MeshRenderer>();
        if (ms.debug)
        {
            destroyRate = 60.0f;
        }
        else
        {
            destroyRate = 10.0f;
        }
    }

    private void Update()
    {
        if (Time.time > bonusTimer)
        {
            bonusTimer = Time.time + bonusRate;
            if (miganie)
            {
                miganie = false;
                mr.material.color = Color.white;
            }
            else
            {
                miganie = true;
                mr.material.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            }
        }
        if (ms.otherTime > destroyTimer)
        {
            Hide();
        }
    }

    private void Hide()
    {
        renderer.enabled = false;
        collider.enabled = false;
        transform.position = Vector3.zero;
    }

    public void Show(Vector3 newPosition, Bonus bonusType)
    {
        destroyTimer = ms.otherTime + destroyRate;
        type = bonusType;
        mr.material.SetTexture("_MainTex", ms.bonusTextures[(int)type]);
        renderer.enabled = true;
        collider.enabled = true;
        transform.position = newPosition;
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (!(otherCollider.tag == "Tank"))
            return;
        TankScript tankScript = otherCollider.transform.GetComponent<TankScript>();
        if (tankScript.ally == "Player")
        {
            if (!ms.noSound)
            {
                audio.Play();
            }
            Player player = ms.playersList[tankScript.number];
            player.scores += 500;
            player.kills[4]++;
            ms.ShowFlower(transform.position, "500");
            switch (type)
            {
                case Bonus.Life:
                {
                    player.lifes++;
                    break;
                }
                case Bonus.Upgrade:
                    if (tankScript.rank < 3)
                    {
                        tankScript.Upgrade(1);
                        break;
                    }
                    else
                        break;
                case Bonus.Bomb:
                    ms.DestroyAllEnemy();
                    break;
                case Bonus.Freeze:
                    ms.FreezeAllEnemy();
                    break;
                case Bonus.Fort:
                    ms.UpgradeFort();
                    break;
                case Bonus.Shield:
                {
                    tankScript.invincible = 10;
                    break;
                }
                case Bonus.FullUpgrade:
                {
                    if (tankScript.rank < 3)
                    {
                        tankScript.Upgrade(3);
                    }
                    break;
                }
                case Bonus.Ship:
                {
                    tankScript.SetWaterWalking(true);
                    break;
                }
            }
            Hide();
        }
        if (!(tankScript.ally == "Enemy") || ms.difficulty != 2)
            return;
        switch (type)
        {
            case Bonus.Life:
                if (tankScript.rank < 3)
                {
                    tankScript.Upgrade(3);
                    break;
                }
                else
                    break;
            case Bonus.Upgrade:
                if (tankScript.rank < 3)
                {
                    tankScript.Upgrade(1);
                    break;
                }
                else
                    break;
            case Bonus.Bomb:
                ms.DestroyAllPlayers();
                break;
            case Bonus.Freeze:
                ms.FreezeAllPlayers();
                break;
            case Bonus.Fort:
                ms.ClearUpgradeFort();
                break;
            case Bonus.Shield:
                if (tankScript.rank < 3)
                {
                    tankScript.Upgrade(3);
                    break;
                }
                else
                    break;
            case Bonus.FullUpgrade:
                if (tankScript.rank < 3)
                {
                    tankScript.Upgrade(3);
                    break;
                }
                else
                    break;
            case Bonus.Ship:
            {
                tankScript.SetWaterWalking(true);
                break;
            }
        }
        Hide();
    }
}
